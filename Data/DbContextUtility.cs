using JobScheduler.Enums;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Data
{
    public class DbContextUtility
    {
        private readonly JobSchedulerContext _context;
        private static IConfiguration _configuration;
        private static string readOut;
        int exitCode = -999;

        public DbContextUtility(JobSchedulerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<bool> Launch(LaunchJob launchJob)
        {
            SlaveJobModel slaveJobModel = new SlaveJobModel
            {
                Id = launchJob.Id,
                Path = launchJob.Path,
                Argomenti = launchJob.Argomenti
            };

            var listGroupes = await _context.JobGroupes
                .Include(jg => jg.Group)
                .Where(jg => jg.JobId == launchJob.Id)
                .Select(jg => jg.Group)
                .ToListAsync();

            IList<Node> listNodes = null;

            if (listGroupes == null || listGroupes.Count < 1)
            {
                //Esecuzione su tutti i nodi esistenti + nodo master è l'unico sul db avente Tipo==0
                Node masterNode = await _context.Nodes.Where(node => node.Tipo == NodeType.Master).FirstAsync();
                slaveJobModel.NodeId = masterNode.Id;
                var masterLaunchResult = MasterLaunchJob(slaveJobModel, _context);

                await SaveLaunchResult(masterNode, slaveJobModel, _context, masterLaunchResult);
                listNodes = await _context.Nodes.ToListAsync();
                await LaunchListNodes(slaveJobModel, listNodes, _context);
            }
            else
            {
                foreach (var group in listGroupes)
                {
                    listNodes = await _context.GroupNodes
                        .Include(gn => gn.Node)
                        .Where(gn => gn.GroupId == group.Id)
                        .Select(gn => gn.Node)
                        .ToListAsync();

                    if (listNodes == null || listNodes.Count < 1)
                    {
                        continue;
                    }

                    await LaunchListNodes(slaveJobModel, listNodes, _context);
                }
            }

            return true;
        }

        private static async Task LaunchListNodes(SlaveJobModel slaveJobModel, IList<Node> listNodes, JobSchedulerContext _context)
        {
            foreach (var node in listNodes)
            {
                //Se nel gruppo associato al job è presente il nodo Master, devo lanciare l'eseguibile direttamente da Master
                if (node.Tipo == NodeType.Master)
                {
                    slaveJobModel.NodeId = node.Id;
                    var masterLaunchResult = MasterLaunchJob(slaveJobModel, _context);
                    await SaveLaunchResult(node, slaveJobModel, _context, masterLaunchResult);
                    continue;
                }

                slaveJobModel.NodeId = node.Id;
                await ExecuteLaunch(node, slaveJobModel, _context);
            }
        }

        private static async Task ExecuteLaunch(Node node, SlaveJobModel slaveJobModel, JobSchedulerContext _context)
        {
            string launchAction = _configuration["SlaveUrls:SlaveLaunch"];
            string slaveUrl = node.IndirizzoIP + launchAction;

            if (string.IsNullOrWhiteSpace(slaveUrl) || slaveJobModel == null)
            {
                return;
            }

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(slaveJobModel), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveUrl}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<JobResult>(apiResponse);
                        await SaveLaunchResult(node, slaveJobModel, _context, result);
                    }
                }
            }
            catch
            {
            }
        }

        private static JobResult MasterLaunchJob(SlaveJobModel slaveJobModel, JobSchedulerContext _context)
        {
            if (slaveJobModel == null
                || slaveJobModel.NodeId == 0)
            {
                return null;
            }
            JobResult jobResult = null;

            try
            {
                using (Process process = new Process())
                {
                    string currentProjectPath = string.Empty;
                    string executablePath = string.Empty;

                    currentProjectPath = UtilityDatabase.GetApplicationRoot();
                    executablePath = _configuration["ExecutableInfo:Path"];

                    string launchPath = Path.Combine(currentProjectPath, executablePath);

                    if (string.IsNullOrWhiteSpace(launchPath))
                    {
                        return jobResult;
                    }

                    process.StartInfo.FileName = launchPath;

                    if (string.IsNullOrWhiteSpace(slaveJobModel.Argomenti) == false)
                    {
                        process.StartInfo.Arguments = $"\"{slaveJobModel.Argomenti}\"";
                    }

                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);

                    jobResult = new JobResult();

                    process.Start();
                    process.BeginOutputReadLine();

                    jobResult.Pid = process.Id;
                    jobResult.IdNode = slaveJobModel.NodeId;

                    //Added to ensure execution of HandleOutputData
                    if (string.IsNullOrWhiteSpace(readOut))
                    {
                        Thread.Sleep(200);
                    }

                    jobResult.StandardOutput = readOut;
                }
            }
            catch (Exception)
            {
                return null;
            }

            return jobResult;
        }

        private static async Task SaveLaunchResult(Node node, SlaveJobModel slaveJobModel, JobSchedulerContext _context, JobResult result)
        {
            if (result != null)
            {
                var x = new LaunchResult();
                await _context.LaunchResult.AddAsync(x);
                await UtilityDatabase.TryCommit<LaunchResult>(_context, x);

                NodeLaunchResult nodeLaunchResult = new NodeLaunchResult
                {
                    JobId = slaveJobModel.Id,
                    NodeId = slaveJobModel.NodeId,
                    Pid = result.Pid,
                    ExitCode = result.ExitCode,
                    StandardOutput = result.StandardOutput,
                    LaunchResult = x,
                    Node = node
                };

                await _context.NodesLaunchResults.AddAsync(nodeLaunchResult);
                await UtilityDatabase.TryCommit<NodeLaunchResult>(_context, nodeLaunchResult);
            }
        }

        private static void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            readOut = e.Data;
        }

        public async Task<bool> Stop(StopJob stopJob)
        {

            var listGroupes = await _context.JobGroupes
                .Include(jg => jg.Group)
                .Where(jg => jg.JobId == stopJob.JobId)
                .Select(jg => jg.Group)
                .ToListAsync();

            if (listGroupes == null || listGroupes.Count < 1)
            {
                var listNodes = await _context.Nodes.ToListAsync();
                foreach (var node in listNodes)
                {
                    var nodeLaunchResult = await _context.NodesLaunchResults.Where(ndl => ndl.NodeId == node.Id && ndl.JobId == stopJob.JobId).FirstOrDefaultAsync();

                    if (nodeLaunchResult == null)
                    {
                        continue;
                    }

                    if (node.Tipo == NodeType.Master)
                    {
                        nodeLaunchResult.Node = node;
                        stopJob.Pid = nodeLaunchResult.Pid;

                        var masterStopResult = MasterStopJob(stopJob);
                        await SaveStopResult(nodeLaunchResult, masterStopResult);
                        continue;
                    }

                    nodeLaunchResult.Node = node;
                    stopJob.Pid = nodeLaunchResult.Pid;
                    await ExecuteStop(nodeLaunchResult, stopJob);
                }
            }
            else
            {
                foreach (var group in listGroupes)
                {
                    var listNodes = await _context.GroupNodes
                        .Include(gn => gn.Node)
                        .Where(gn => gn.GroupId == group.Id)
                        .Select(gn => gn.Node)
                        .ToListAsync();

                    if (listNodes == null || listNodes.Count < 1)
                    {
                        continue;
                    }

                    foreach (var node in listNodes)
                    {
                        var nodeLaunchResultList = await _context.NodesLaunchResults.Where(ndl => ndl.NodeId == node.Id && ndl.JobId == stopJob.JobId && ndl.ExitCode == 0).ToListAsync();

                        if (nodeLaunchResultList == null || nodeLaunchResultList.Count < 1)
                        {
                            continue;
                        }

                        foreach (var nodeLaunchResult in nodeLaunchResultList)
                        {

                            if (node.Tipo == NodeType.Master)
                            {
                                nodeLaunchResult.Node = node;
                                stopJob.Pid = nodeLaunchResult.Pid;

                                var masterStopResult = MasterStopJob(stopJob);
                                await SaveStopResult(nodeLaunchResult, masterStopResult);
                                continue;
                            }

                            nodeLaunchResult.Node = node;
                            stopJob.Pid = nodeLaunchResult.Pid;
                            await ExecuteStop(nodeLaunchResult, stopJob);
                        }

                    }
                }
            }

            return true;
        }

        public JobResult MasterStopJob(StopJob stopJob)
        {
            bool killProcessTree = false;
            JobResult jobResult = null;

            if (stopJob.Pid == 0)
            {
                return jobResult;
            }

            try
            {
                var processFound = Process.GetProcessById(stopJob.Pid);
                processFound.EnableRaisingEvents = true;
                processFound.Exited += ProcessEnded;
                processFound.Kill(killProcessTree);
                processFound.WaitForExit();

                if (processFound != null)
                {
                    //Added to ensure execution of ProcessEnded
                    if (exitCode == -999)
                    {
                        Thread.Sleep(200);
                    }

                    jobResult = new JobResult
                    {
                        Pid = stopJob.Pid,
                        ExitCode = exitCode
                    };
                }
            }
            catch (Exception)
            {
                return null;
            }

            return jobResult;
        }
      
        public async Task ExecuteStop(NodeLaunchResult nodeLaunchResult, StopJob stopJob)
        {
            string launchAction = _configuration["SlaveUrls:SlaveStop"];
            string slaveUrl = nodeLaunchResult.Node.IndirizzoIP + launchAction;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(stopJob), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveUrl}", content))
                    {
                        if (response == null || response.Content == null)
                        {
                            return;
                        }

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<JobResult>(apiResponse);

                        await SaveStopResult(nodeLaunchResult, result);
                    }
                }
            }
            catch
            {
            }

            return;
        }

        private async Task SaveStopResult(NodeLaunchResult nodeLaunchResult, JobResult result)
        {
            var nodeLaunchResultFound = (from ndl in _context.NodesLaunchResults
                                         where ndl.JobId == nodeLaunchResult.JobId
                                         && ndl.NodeId == nodeLaunchResult.NodeId
                                         && ndl.Pid == nodeLaunchResult.Pid
                                         select ndl).FirstOrDefault();

            if (nodeLaunchResultFound == null || result == null)
            {
                return;
            }

            //TODO: gestire il ritorno?
            nodeLaunchResultFound.ExitCode = result.ExitCode;

            _context.NodesLaunchResults.Update(nodeLaunchResultFound);

            await UtilityDatabase.TryCommit<NodeLaunchResult>(_context, nodeLaunchResultFound);

            return;
        }

        private void ProcessEnded(object sender, EventArgs e)
        {
            var process = sender as Process;
            if (process != null)
            {
                exitCode = process.ExitCode;
            }
        }

    }
}

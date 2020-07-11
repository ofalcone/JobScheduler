using JobScheduler.Enums;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private static string readOut;
        private readonly JobSchedulerContext _context;
        private static IConfiguration _configuration;

        public DbContextUtility(JobSchedulerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<object> Launch(LaunchJob launchJob)
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
                //Esecuzione su tutti i nodi esistenti
                var masterLaunchResult = MasterLaunchJob(slaveJobModel, _context);
                Node masterNode = await _context.Nodes.Where(node => node.Tipo == NodeType.Master).FirstAsync();
                await SaveLaunchResult(masterNode,slaveJobModel, _context,masterLaunchResult);
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

            return default;
        }

        private static async Task LaunchListNodes(SlaveJobModel slaveJobModel, IList<Node> listNodes, JobSchedulerContext _context)
        {
            foreach (var node in listNodes)
            {
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

                //Il risultato che viene scritto nella JobGroup sarebbe quello dell'esecuzione dell'ultimo nodo -> non utile
                //if (groupId != 0)
                //{
                //    var jobGroup = await _context.JobGroupes.FindAsync(slaveJobModel.Id, groupId);
                //    jobGroup.LastExecutionDate = DateTime.Now;
                //    jobGroup.Pid = result.Pid;
                //    jobGroup.OutputResult = result.StandardOutput;
                //    jobGroup.ExecutionResult = Enums.ExecutionEnum.Success;
                //}


                await _context.NodesLaunchResults.AddAsync(nodeLaunchResult);
                await UtilityDatabase.TryCommit<NodeLaunchResult>(_context, nodeLaunchResult);
            }
        }

        private static JobResult MasterLaunchJob(SlaveJobModel slaveJobModel, JobSchedulerContext _context)
        {
            if (slaveJobModel == null
                || string.IsNullOrWhiteSpace(slaveJobModel.Path)
                || slaveJobModel.NodeId == 0)
            {
                return null;
            }
            JobResult jobResult = new JobResult();

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = slaveJobModel.Path;
                    if (string.IsNullOrWhiteSpace(slaveJobModel.Argomenti) == false)
                    {
                        process.StartInfo.Arguments = $"\"{slaveJobModel.Argomenti}\"";
                    }
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;

                    process.OutputDataReceived += new DataReceivedEventHandler(HandleOutputData);
                    //process.OutputDataReceived += (sender, args) => readOut = args.Data;
                    process.Start();
                    process.BeginOutputReadLine();
                    //readOut = process.StandardOutput.ReadToEnd();

                    //var output = new List<string>();

                    //while (process.StandardOutput.Peek() > -1)
                    //{
                    //    output.Add(process.StandardOutput.ReadLine());
                    //}

                    jobResult.Pid = process.Id;
                    jobResult.IdNode = slaveJobModel.NodeId;
                    if (string.IsNullOrWhiteSpace(readOut))
                    {
                        Thread.Sleep(500);
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

        private static void HandleOutputData(object sender, DataReceivedEventArgs e)
        {
            readOut = e.Data;
        }

        public async Task<IActionResult> Stop(StopJob stopJob)
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

                    nodeLaunchResult.Node = node;
                    stopJob.Pid = nodeLaunchResult.Pid;
                    return await ExecuteStop(nodeLaunchResult, stopJob);
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
                            nodeLaunchResult.Node = node;
                            stopJob.Pid = nodeLaunchResult.Pid;
                            return await ExecuteStop(nodeLaunchResult, stopJob);
                        }
                        
                    }
                }
            }

            return null;
        }


        public async Task<IActionResult> ExecuteStop(NodeLaunchResult nodeLaunchResult, StopJob stopJob)
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
                            return null;
                        }

                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<JobResult>(apiResponse);

                        var nodeLaunchResultFound = (from ndl in _context.NodesLaunchResults
                                     where ndl.JobId == nodeLaunchResult.JobId
                                     && ndl.NodeId == nodeLaunchResult.NodeId
                                     && ndl.Pid == nodeLaunchResult.Pid
                                     select ndl).FirstOrDefault();

                        if (nodeLaunchResultFound == null)
                        {
                            return null;
                        }

                        //TODO: gestire il ritorno?
                        nodeLaunchResultFound.ExitCode = result.ExitCode;

                        _context.NodesLaunchResults.Update(nodeLaunchResultFound);

                        await UtilityDatabase.TryCommit<NodeLaunchResult>(_context, nodeLaunchResultFound);
                    }
                }
            }
            catch
            {
            }

            return null;
        }

     
    }
}

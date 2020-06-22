using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace JobScheduler.Data
{
    public class DbContextUtility
    {
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
                listNodes = await _context.Nodes.ToListAsync();
                await LaunchListNodes(slaveJobModel, listNodes,_context);
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
                await ExecuteLaunch(node.IndirizzoIP, slaveJobModel, _context);
            }
        }

        private static async Task ExecuteLaunch(string slaveIp, SlaveJobModel slaveJobModel, JobSchedulerContext _context)
        {
            string launchAction = _configuration["SlaveUrls:SlaveLaunch"];
            string slaveUrl = slaveIp + launchAction;

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

                        if (result != null)
                        {
                            NodeLaunchResult nodeLaunchResult = new NodeLaunchResult
                            {
                                JobId = slaveJobModel.Id,
                                NodeId = slaveJobModel.NodeId,
                                Pid = result.Pid,
                                ExitCode = result.ExitCode,
                                StandardOutput = result.StandardOutput,
                            };

                            await _context.NodesLaunchResults.AddAsync(nodeLaunchResult);
                        }
                        //return result;
                        //scrivere su db le info ritornate dallo slave
                    }
                }
            }
            catch
            {
            }
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
            }

            return null;
        }


        public async Task<IActionResult> ExecuteStop(NodeLaunchResult nodeLaunchResult, StopJob stopJob)
        {
            string launchAction = _configuration["SlaveUrls:SlaveLaunch"];
            string slaveUrl = nodeLaunchResult.Node.IndirizzoIP + launchAction;
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(stopJob), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveUrl}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<IActionResult>(apiResponse);
                        return result;
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

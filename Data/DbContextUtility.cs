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
        private readonly IConfiguration _configuration;

        public DbContextUtility(JobSchedulerContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<object> Launch(LaunchJob launchJob)
        {
            string slaveURl = string.Format(_configuration["SlaveUrls:Slave1-Launch"]);
            SlaveJobModel test = new SlaveJobModel
            {
                Id = launchJob.Id,
                Path = launchJob.Path,
                Argument = ""
            };

            var listGroupes = await _context.JobGroupes
                .Include(jg => jg.Group)
                .Where(jg => jg.JobId == launchJob.Id)
                .Select(jg => jg.Group)
                .ToListAsync();

            if (listGroupes == null || listGroupes.Count < 1)
            {
                //Esecuzione su tutti i nodi esistenti
                test.IdNodeList = _context.Nodes.Select(node => node.Id).ToList();
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

                    test.IdNodeList = listNodes.Select(node => node.Id).ToList();
                }
            }

            if (test.IdNodeList == null || test.IdNodeList.Count < 1)
            {
                return null;
            }

            await ExecuteLaunch(slaveURl, test);
            return default;
        }

        private static async Task ExecuteLaunch(string slaveURl, SlaveJobModel test)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(test), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveURl}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JobResult>>(apiResponse);

                        if (result != null && result.Count > 0)
                        {

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
            string slaveURl = string.Format(_configuration["SlaveUrls:Slave1-Stop"]);

            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(stopJob), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveURl}", content))
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
            return default;
        }
    }
}

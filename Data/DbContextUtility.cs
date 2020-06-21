using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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

            if (listGroupes == null || listGroupes.Count<1)
            {
                return null;
            }

            foreach (var group in listGroupes)
            {
                var listNodes = await _context.GroupNodes
                    .Include(gn => gn.Node)
                .Where(gn => gn.GroupId == group.Id)
                .Select(gn => gn.Node)
                .ToListAsync();

                test.IdNodeList = listNodes.Select(node => node.Id).ToList();
            }

            if (test.IdNodeList == null || test.IdNodeList.Count<1)
            {
                return null;
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(test), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync($"{slaveURl}", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<JobResult>>(apiResponse);

                        if (result == null)
                        {
                            return null;
                        }

                        //scrivere su db le info ritornate dallo slave
                    }
                }
            }
            catch
            {
            }
            return default;
        }

        public async Task<object> Stop(StopJob stopJob)
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
                        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(apiResponse);
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

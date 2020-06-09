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
            string slaveURl = string.Format(_configuration["SlaveUrls:Slave1-Start"]);
            SlaveJobModel test = new SlaveJobModel
            {
                Id = launchJob.Id,
                Path = launchJob.Path,
                Argument = ""

            };

            //var list = await _context.Jobs.Include(job => job.JobGroupes)
            //    .ThenInclude(jobGroupes => jobGroupes.Group)
            //    .Where(job => job.Id == launchJob.Id)
            //    .ToListAsync();

            var listGroupes = from job in _context.Jobs
                              from gr in _context.Groups
                              where job.Id == launchJob.Id
                              select gr;

            foreach (var item in listGroupes)
            {
                var listNodes = from gr in _context.Groups
                                from nodes in _context.Nodes
                                where gr.Id == item.Id
                                select nodes.Id;
                test.IdNodeList = listNodes.ToList<int>();
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(test), Encoding.UTF8, "application/json");
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

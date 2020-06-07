using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
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

        public DbContextUtility(JobSchedulerContext context)
        {
            _context = context;
        }

        public async Task<object> Launch(LaunchJob launchJob)
        {
            string slaveURl = string.Format("https://localhost:5004/api/JobExe");
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
                using (var httpClient = new System.Net.Http.HttpClient())
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
    }
}

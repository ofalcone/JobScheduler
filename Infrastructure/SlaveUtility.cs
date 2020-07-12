using JobScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using JobScheduler.Data;
using SQLitePCL;

namespace JobScheduler.Infrastructure
{
    public static class SlaveUtility<TContext>
        where TContext : JobSchedulerContext
    {
        internal static async Task<object> CallSlave(LaunchJob launchJob, TContext context)
        {
            string slaveURl = string.Format("https://localhost:5004/api/JobExe");
            object test = new
            {
                Id = launchJob.Id,
                Path = launchJob.Path,
                Argument = "",
                IdNodeList = new List<int> { 1, 2, 3 }

            };

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

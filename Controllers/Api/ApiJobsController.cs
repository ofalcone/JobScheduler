using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;
using System.Net.Http;
using System.Text;
using JobScheduler.Infrastructure;
using JobScheduler.Abstract;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiJobsController : CrudController<JobSchedulerContext, Job>
    {

        public ApiJobsController(JobSchedulerContext context) : base(context)
        {
        }
     
        ////TODO: Spostare in utilityclass
        public static async Task<ActionResult<object>> Launch(LaunchJob launchJob)
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

        //// DELETE: api/ApiJobs/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Job>> DeleteJob(int id)
        //{
        //    var job = await _context.Jobs.FindAsync(id);
        //    if (job == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Jobs.Remove(job);
        //    await _context.SaveChangesAsync();

        //    return job;
        //}

        //private bool JobExists(int id)
        //{
        //    return _context.Jobs.Any(e => e.Id == id);
        //}
    }
}

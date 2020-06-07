using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using JobScheduler.Infrastructure;
using JobScheduler.Abstract;

namespace JobScheduler.Controllers
{
    [Authorize]
    public class JobsController : MvcCrudController<JobSchedulerContext,Job>
    {
        private readonly JobSchedulerContext _context;
        public JobsController(JobSchedulerContext context) : base(context)
        {
            _context = context;
        }

        //public static async Task<ActionResult<object>> Launch(LaunchJob launchJob)
        //{
        //    string slaveURl = string.Format("https://localhost:5004/api/JobExe");
        //    object test = new
        //    {
        //        Id = launchJob.Id,
        //        Path = launchJob.Path,
        //        Argument = "",
        //        IdNodeList = new List<int> { 1, 2, 3 }

        //    };
        //    try
        //    {
        //        using (var httpClient = new System.Net.Http.HttpClient())
        //        {
        //            StringContent content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(test), Encoding.UTF8, "application/json");
        //            using (var response = await httpClient.PostAsync($"{slaveURl}", content))
        //            {
        //                string apiResponse = await response.Content.ReadAsStringAsync();
        //                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<object>(apiResponse);
        //                return result;
        //            }
        //        }
        //    }
        //    catch
        //    {
        //    }
        //    return default;
        //}
        public async Task<object> Launch(LaunchJob launchJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context);
            return await dbContextUtility.Launch(launchJob);
        }
    }
}

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
using Microsoft.Extensions.Configuration;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiJobsController : CrudController<JobSchedulerContext, Job>
    {
        private readonly JobSchedulerContext _context;
        private readonly IConfiguration _configuration;
        //private SlaveUtility slaveUtility;
        public ApiJobsController(JobSchedulerContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            //slaveUtility = new SlaveUtility(context);
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<object> Launch(LaunchJob launchJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            return await dbContextUtility.Launch(launchJob);
        }

        [HttpPost]
        public async Task<object> Stop(StopJob stopJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            return await dbContextUtility.Stop(stopJob);
        }

    }
}

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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers.Api
{
    [Authorize(Roles = Constants.ADMIN_ROLE + "," + Constants.EDITOR_ROLE, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpPost("[action]")]
        public async Task<object> Launch(LaunchJob launchJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            return await dbContextUtility.Launch(launchJob);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Stop(StopJob stopJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            return await dbContextUtility.Stop(stopJob);
        }

    }
}

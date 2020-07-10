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
using Microsoft.Extensions.Configuration;

namespace JobScheduler.Controllers
{
    [Authorize]
    public class JobsController : MvcCrudController<JobSchedulerContext,Job>
    {
        private readonly JobSchedulerContext _context;
        private readonly IConfiguration _configuration;
        public JobsController(JobSchedulerContext context, IConfiguration configuration) : base(context, configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        //Scrivere da qualche parte le info di ritorno
        public async Task<object> Launch(LaunchJob launchJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context,_configuration);
            return await dbContextUtility.Launch(launchJob);
        }

        //Dove prendo il pid da killare? ho il job id ma manca il groupId dove è stato eseguito
        public async Task<object> Stop(StopJob stopJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            return await dbContextUtility.Stop(stopJob);
        }
    }
}

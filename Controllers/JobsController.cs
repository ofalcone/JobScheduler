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
        public JobsController(JobSchedulerContext context, IConfiguration configuration) : base(context)
        {
            _context = context;
            _configuration = configuration;
        }


        //TODO: definire oggetto di ritorno
        public async Task<object> Launch(LaunchJob launchJob)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context,_configuration);
            return await dbContextUtility.Launch(launchJob);
        }

        //TODO: definire oggetto di ritorno
        public async Task<object> Stop(int jobId)
        {
            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            StopJob stopJob = null;
            //TODO: fare un metodo che prende il PID del programma da arrestare (ritornato dalla Launch) dato l'id del job (Dove lo mettiamo il PID?)
            //stopJob = dbContextUtility.GetStopJobById(jobId);
            return await dbContextUtility.Stop(stopJob);
        }
    }
}

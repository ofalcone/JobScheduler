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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE + "," + Constants.EDITOR_ROLE)]
    public class JobsController : MvcCrudController<JobSchedulerContext, Job>
    {
        private readonly JobSchedulerContext _context;
        private readonly IConfiguration _configuration;
        //private readonly IServiceScopeFactory _scopeFactory;
        public JobsController(JobSchedulerContext context, IConfiguration configuration, IServiceScopeFactory _scopeFactory)
            : base(context, _scopeFactory, configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Launch(LaunchJob launchJob)
        {

            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            await dbContextUtility.Launch(launchJob);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Stop(StopJob stopJob)
        {
            if (stopJob.JobId == 0 )
            {
                //Gestirla con una schermata di errore ad hoc
                return RedirectToAction(nameof(Index));
            }

            DbContextUtility dbContextUtility = new DbContextUtility(_context, _configuration);
            await dbContextUtility.Stop(stopJob);
            return RedirectToAction(nameof(Index));

        }
    }
}

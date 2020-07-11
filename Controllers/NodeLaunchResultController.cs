using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE + "," + Constants.EDITOR_ROLE)]
    public class NodeLaunchResultController : Controller
    {

        private readonly JobSchedulerContext _context;
        private readonly NodeLaunchResultUtility _nodeLaunchResultUtility;

        //public JobGroupsController(JobSchedulerContext context):base(context)
        public NodeLaunchResultController(JobSchedulerContext context)
        {
            _context = context;
            _nodeLaunchResultUtility = new NodeLaunchResultUtility(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _nodeLaunchResultUtility.GetAll());
        }
    }
}
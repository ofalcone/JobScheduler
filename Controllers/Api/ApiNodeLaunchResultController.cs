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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Authorize(Roles = Constants.ADMIN_ROLE + "," + Constants.EDITOR_ROLE, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class ApiNodeLaunchResultController : ControllerBase
    {
        private readonly JobSchedulerContext _context;
        private readonly NodeLaunchResultUtility _nodeLaunchResultUtility;

        //public JobGroupsController(JobSchedulerContext context):base(context)
        public ApiNodeLaunchResultController(JobSchedulerContext context)
        {
            _context = context;
            _nodeLaunchResultUtility = new NodeLaunchResultUtility(context);
        }

        [HttpGet]
        public async Task<IEnumerable<NodeLaunchResult>> Get()
        {
            return await _nodeLaunchResultUtility.GetAll();
        }
    }
}
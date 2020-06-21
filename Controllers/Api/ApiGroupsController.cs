using System;
using System.Collections.Generic;
using System.Linq;
using JobScheduler.Models;
using System.Threading.Tasks;
using JobScheduler.Abstract;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class ApiGroupsController : CrudController<JobSchedulerContext, Group>
    {
        public ApiGroupsController(JobSchedulerContext context) : base(context)
        {
        }
    }

}
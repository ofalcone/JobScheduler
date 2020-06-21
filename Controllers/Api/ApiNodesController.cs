using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Abstract;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
    public class ApiNodesController : CrudController<JobSchedulerContext,Node >
    {
        public ApiNodesController(JobSchedulerContext context) : base(context)
        {
        }
    }
}

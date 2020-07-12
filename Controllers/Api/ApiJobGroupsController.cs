using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ADMIN_ROLE + "," + Constants.EDITOR_ROLE, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiJobGroupsController : ControllerBase
    {
        private readonly JobSchedulerContext _context;
        private readonly JobGroupsUtility _jobGroupsUtility;

        public ApiJobGroupsController(JobSchedulerContext context)
        {
            _context = context;
            _jobGroupsUtility = new JobGroupsUtility(context);
        }


        [HttpGet]
        public async Task<IEnumerable<JobGroup>> Get()
        {
            return await _jobGroupsUtility.GetAll();
        }


        [HttpGet]
        public async Task<JobGroup> Get(JobGroup jobGroup)
        {
            return await _jobGroupsUtility.GetSingle(jobGroup);
        }


        [HttpPost]
        public async Task Post(JobGroup jobGroup)
        {
            await _jobGroupsUtility.CreateSingle(jobGroup);
        }


        [HttpPut]
        public async Task Put(JobGroupViewModel jobGroupViewModel)
        {
            await _jobGroupsUtility.Update(jobGroupViewModel);
        }


        [HttpDelete]
        public async Task Delete(JobGroup jobGroup)
        {
            await _jobGroupsUtility.Delete(jobGroup);
        }
    }
}
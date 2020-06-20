using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiJobGroupsController : ControllerBase
    {

        //TODO gestire errori

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
            //TODO: questa get probabilmente non ha senso...
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
            //TODO cambiare input type con JobGroup oppure usare sempre JobGroupViewModel
            await _jobGroupsUtility.Update(jobGroupViewModel);
        }


        [HttpDelete]
        public async Task Delete(JobGroup jobGroup)
        {
            await _jobGroupsUtility.Delete(jobGroup);
        }
    }
}
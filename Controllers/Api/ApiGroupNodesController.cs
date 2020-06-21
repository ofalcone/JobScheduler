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

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
    public class ApiGroupNodesController : ControllerBase
    {

        private readonly JobSchedulerContext _context;
        private readonly GroupNodesUtility _groupNodesUtility;

        public ApiGroupNodesController(JobSchedulerContext context)
        {
            _context = context;
            _groupNodesUtility = new GroupNodesUtility(context);
        }


        [HttpGet]
        public async Task<IEnumerable<GroupNode>> Get()
        {
            return await _groupNodesUtility.GetAll();
        }


        [HttpGet]
        public async Task<GroupNode> Get(GroupNode jobGroup)
        {
            //TODO: questa get probabilmente non ha senso...
            return await _groupNodesUtility.GetSingle(jobGroup);
        }


        [HttpPost]
        public async Task Post(GroupNode groupNode)
        {
            await _groupNodesUtility.CreateSingle(groupNode);
        }


        [HttpPut]
        public async Task Put(GroupNodeViewModel groupNodeViewModel)
        {
            //TODO cambiare input type con GroupNode oppure usare sempre GroupNodeViewModel
            await _groupNodesUtility.Update(groupNodeViewModel);
        }


        [HttpDelete]
        public async Task Delete(GroupNode groupNode)
        {
            await _groupNodesUtility.Delete(groupNode);
        }
    }
}
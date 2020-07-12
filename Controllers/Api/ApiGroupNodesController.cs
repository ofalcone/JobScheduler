using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ADMIN_ROLE, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ApiGroupNodesController : ControllerBase
    {
        private readonly GroupNodesUtility _groupNodesUtility;

        public ApiGroupNodesController(JobSchedulerContext context)
        {
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
            await _groupNodesUtility.Update(groupNodeViewModel);
        }


        [HttpDelete]
        public async Task Delete(GroupNode groupNode)
        {
            await _groupNodesUtility.Delete(groupNode);
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ADMIN_ROLE,AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ApiUsersController : ControllerBase
    {
        private readonly UserUtility _userUtility;

        public ApiUsersController(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userUtility = new UserUtility(userManager, context);
        }


        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userUtility.GetUsers();
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            UserViewModel user = await _userUtility.GetUserById(id);

            return Ok(user);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserViewModel userView)
        {
            if (userView==null)
            {
                return BadRequest();
            }

            string errorResult = await _userUtility.Create(userView);

            if (string.IsNullOrWhiteSpace(errorResult))
            {
                return Ok();
            }

            return BadRequest(errorResult);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UserViewModel userView)
        {
            if (userView == null)
            {
                return BadRequest();
            }

            IdentityResult result = await _userUtility.Update(userView);
            if (result == null || result.Succeeded == false)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            IdentityResult result = await _userUtility.Delete(id);
            if (result == null || result.Succeeded == false)
            {
                return NotFound();
            }

            return Ok();
        }
    }

}

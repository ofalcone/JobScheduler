using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = RolesNames.Admin)]
    public class ApiRolesController : ControllerBase
    {
        //TODO: controllare sempre il ruolo dal token JWT: solo un admin può modificare le tabelle di Roles e Users

        private readonly RolesUtility _rolesUtility;

        public ApiRolesController(RoleManager<IdentityRole> roleManager, JobSchedulerContext context)
        {
            _rolesUtility = new RolesUtility(roleManager, context);
        }


        [HttpGet]
        public IEnumerable<IdentityRole> Get()
        {
            return _rolesUtility.GetRoles();
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            IdentityRole role = await _rolesUtility.GetRoleById(id);

            return Ok(role);
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IdentityRole role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            string errorResult = await _rolesUtility.Create(role);

            if (string.IsNullOrWhiteSpace(errorResult))
            {
                return Ok();
            }

            return BadRequest(errorResult);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] IdentityRole role)
        {
            if (role == null)
            {
                return BadRequest();
            }

            IdentityResult result = await _rolesUtility.Update(role);
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

            IdentityResult result = await _rolesUtility.Delete(id);
            if (result == null || result.Succeeded == false)
            {
                return NotFound();
            }

            return Ok();
        }

    }
}

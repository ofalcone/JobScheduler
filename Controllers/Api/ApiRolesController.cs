using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiRolesController : ControllerBase
    {

        //TODO: completare i metodi affidandosi a RolesUtility + pensare se è corretto istanziare UserUtility nel costruttore
        //TODO: controllare sempre il ruolo dal token JWT: solo un admin può modificare le tabelle di Roles e Users
        //private readonly RoleManager<IdentityRole> _roleManager;
        //private JobSchedulerContext _context;
        private readonly RolesUtility _rolesUtility;

        public ApiRolesController(RoleManager<IdentityRole> roleManager, JobSchedulerContext context)
        {
            //_roleManager = roleManager;
            //_context = context;
            _rolesUtility = new RolesUtility(roleManager, context);
        }

        // GET: api/<RolesController>
        [HttpGet]
        public IEnumerable<IdentityRole> Get()
        {
            return _rolesUtility.GetRoles();
        }


        // GET api/<ApiRolesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            IdentityRole role = await _rolesUtility.GetRoleById(id);

            return Ok(role);
        }

        // POST api/<UsersController>
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

        // PUT api/<UsersController>/5
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

        // DELETE api/<UsersController>/5
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

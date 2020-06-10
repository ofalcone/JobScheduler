using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiUsersController : ControllerBase
    {
        //TODO: completare i metodi affidandosi a UserUtility + pensare se è corretto istanziare UserUtility nel costruttore

        private readonly UserUtility _userUtility;

        public ApiUsersController(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userUtility = new UserUtility(userManager, context);
        }

        // GET: api/<UsersController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userUtility.GetUsers();
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserViewModel userView)
        {
            string errorResult = await _userUtility.Create(userView);

            if (string.IsNullOrWhiteSpace(errorResult))
            {
                return Ok();
            }

            return BadRequest(errorResult);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromBody] UserViewModel userView)
        {
            IdentityResult result = await _userUtility.Update(userView);

            if (result!=null)
            {
                return Ok();
            }

            return BadRequest();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            IdentityResult result = await _userUtility.Delete(id);

            if (result != null)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}

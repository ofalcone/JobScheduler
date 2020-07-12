using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using JobScheduler.Infrastructure;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public ApiAccountController(IConfiguration configuration, UserManager<User> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            TokenModel tokenModel = await AccountUtility.GenerateToken(model, _userManager, _configuration);
            if (tokenModel == null)
            {
                return BadRequest();
            }

            return Ok(tokenModel);
        }
    }
}

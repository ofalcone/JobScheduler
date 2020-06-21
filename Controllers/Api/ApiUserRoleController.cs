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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class ApiUserRoleController : ControllerBase
    {
        private readonly JobSchedulerContext _context;
        private readonly RolesUtility _roleUtility;
        private readonly UserUtility _userUtility;
        private readonly UserRoleUtility _userRoleUtility;

        public ApiUserRoleController(JobSchedulerContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userUtility = new UserUtility(userManager, context);
            _roleUtility = new RolesUtility(roleManager, context);
            _userRoleUtility = new UserRoleUtility(_userUtility, _roleUtility, _context);
        }


        [HttpGet]
        public async Task<IEnumerable<UserRoleViewModel>> Get()
        {
            return await _userRoleUtility.GetUserRoles();
        }


        [HttpGet]
        public async Task<UserRoleViewModel> Get(string userId, string roleId)
        {
            return await _userRoleUtility.GetSingle(userId, roleId);
        }


        [HttpPost]
        public async Task Post([FromBody] string userId, string roleId)
        {
            await _userRoleUtility.CreateUserRole(userId, roleId);
        }


        [HttpPut]
        public async Task Put([FromBody] string userId, string roleId)
        {
            await _userRoleUtility.Update(userId, roleId);
        }


        [HttpDelete]
        public async Task Delete(string userId, string roleId)
        {
            await _userRoleUtility.DeleteUserRole(userId, roleId);
        }
    }
}

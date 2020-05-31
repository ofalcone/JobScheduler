using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using JobScheduler.ViewModels;
using JobScheduler.Infrastructure;

namespace Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RolesController_ : ControllerBase
    {



        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController_(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<ActionResult<JobScheduler.ViewModels.QueryResponse<IdentityRole>>> Get([FromQuery] RoleQuery query = null)
        {
            return await _roleManager.Roles.ToQueryResponse(query);
        }

        [HttpPost]
        public async Task<ActionResult<IdentityRole>> Post([FromBody] IdentityRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return role;
            }
            else
            {
                throw Error.Identity(result.Errors);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<IdentityRole>> Put(string id, [FromBody] IdentityRole role)
        {
            var r = await _roleManager.FindByIdAsync(id);
            if (r == null) return NotFound();

            r.Name = role.Name;
            var result = await _roleManager.UpdateAsync(r);

            if (result.Succeeded)
            {
                return r;
            }
            else
            {
                throw Error.Identity(result.Errors);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<IdentityRole>> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return role;
            }
            else
            {
                throw Error.Identity(result.Errors);
            }
        }
    }
}

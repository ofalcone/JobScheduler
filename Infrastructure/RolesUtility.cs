using JobScheduler.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class RolesUtility
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JobSchedulerContext _context;

        public RolesUtility(RoleManager<IdentityRole> roleManager, JobSchedulerContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        internal IEnumerable<IdentityRole> GetRoles()
        {
            return _roleManager.Roles.ToArray();
        }

        internal async Task<string> Create(IdentityRole role)
        {
            string errorString = "";

            IdentityRole newUser = new IdentityRole
            {
               Name= role.Name
            };


            var result = await _roleManager.CreateAsync(newUser);

            if (result.Succeeded == false)
            {
                errorString = result.Errors.FirstOrDefault<IdentityError>().ToString();
                return errorString;
            }

            //TODO: controllare se _roleManager.CreateAsync fa già la commit (probabilmente sì)
            await UtilityDatabase.TryCommit<IdentityRole>(_context, newUser);

            return errorString;
        }

        internal async Task<IdentityRole> GetUserById(string id)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return null;
            }

            return role;
        }

        internal async Task<IdentityResult> Update(IdentityRole role)
        {
            IdentityRole updateRole = await _roleManager.FindByIdAsync(role.Id);

            if (updateRole == null)
            {
                return null;
            }
            updateRole.Name = role.Name;

            IdentityResult identityResult = await _roleManager.UpdateAsync(updateRole);
            return identityResult;
        }

        internal async Task<IdentityResult> Delete(string id)
        {
            IdentityResult result = null;
            IdentityRole user = await _roleManager.FindByIdAsync(id);

            if (user == null)
            {
                return result;
            }

            result = await _roleManager.DeleteAsync(user);
            return result;
        }
    }
}

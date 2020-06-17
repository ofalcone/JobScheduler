using JobScheduler.Data;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class UserRoleUtility
    {
        private UserUtility _userUtility;
        private RolesUtility _roleUtility;
        private JobSchedulerContext _context;

        public UserRoleUtility(UserUtility userUtility, RolesUtility roleUtility, JobSchedulerContext context)
        {
            _userUtility = userUtility;
            _roleUtility = roleUtility;
            _context = context;
        }

        internal async Task<List<UserRoleViewModel>> GetUserRoles()
        {
            List<UserRoleViewModel> userRoleViewModels = new List<UserRoleViewModel>();

            var userRoles = _context.UserRoles.ToArray();
            foreach (var item in userRoles)
            {
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel
                {
                    UserId = item.UserId,
                    User = await _userUtility.GetUserById(item.UserId),
                    RoleId = item.RoleId,
                    Role = await _roleUtility.GetRoleById(item.RoleId)
                };

                userRoleViewModels.Add(userRoleViewModel);
            }

            return userRoleViewModels;
        }

        internal async Task<UserRoleViewModel> GetUserRole(string userId, string roleId)
        {
            var item = await _context.UserRoles.FindAsync(userId, roleId);
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel
            {
                UserId = item.UserId,
                User = await _userUtility.GetUserById(item.UserId),
                RoleId = item.RoleId,
                Role = await _roleUtility.GetRoleById(item.RoleId)
            };
            return userRoleViewModel;
        }

        internal async Task CreateUserRole(string userId, string roleId)
        {
            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(identityUserRole);
            await _context.SaveChangesAsync();
        }

        internal async Task<UserRoleViewModel> GetSingle(string userId, string roleId)
        {
            var item = await _context.UserRoles.FindAsync(userId, roleId);
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel
            {
                UserId = item.UserId,
                User = await _userUtility.GetUserById(item.UserId),
                RoleId = item.RoleId,
                Role = await _roleUtility.GetRoleById(item.RoleId)
            };
            return userRoleViewModel;
        }

        internal async Task Update(string userId, string roleId)
        {
            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Update(identityUserRole);
            await _context.SaveChangesAsync();
        }

        internal async Task DeleteUserRole(string userId, string roleId)
        {
            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Remove(identityUserRole);
            await _context.SaveChangesAsync();
        }

    }
}

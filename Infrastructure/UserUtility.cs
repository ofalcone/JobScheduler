using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class UserUtility
    {
        private readonly UserManager<User> _userManager;
        private readonly JobSchedulerContext _context;

        public UserUtility(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        internal IEnumerable<User> GetUsers()
        {
            return _userManager.Users.ToArray();
        }

        internal async Task<string> Create(UserViewModel userView)
        {
            string errorString = "";

            User newUser = new User
            {
                UserName = userView.Email,
                Email = userView.Email,
                FirstName = userView.FirstName,
                LastName = userView.LastName
            };

            string password = userView.Password;

            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded == false)
            {
                errorString = result.Errors.FirstOrDefault<IdentityError>().ToString();
                return errorString;
            }

            //TODO: controllare se _userManager.CreateAsync fa già la commit (probabilmente sì)
            await UtilityDatabase.TryCommit<User>(_context, newUser);

            return errorString;
        }

        internal async Task<User> GetUserById(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            return user;
        }

        internal async Task<IdentityResult> Update(UserViewModel userView)
        {
            User updateUser = new User
            {
                UserName = userView.Email,
                Email = userView.Email,
                FirstName = userView.FirstName,
                LastName = userView.LastName,
                PasswordHash = userView.Password
            };

            IdentityResult identityResult = await _userManager.UpdateAsync(updateUser);
            return identityResult;
        }

        internal async Task<IdentityResult> Delete(int id)
        {
            IdentityResult result = null;
            User user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
            {
                return result;
            }

            result = await _userManager.DeleteAsync(user);
            return result;
        }
    }
}

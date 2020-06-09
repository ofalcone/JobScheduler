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
        private UserManager<User> _userManager;
        private JobSchedulerContext _context;

        public UserUtility(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        internal IEnumerable<User> GetUsers()
        {
            return _userManager.Users.ToArray();
        }
    }
}

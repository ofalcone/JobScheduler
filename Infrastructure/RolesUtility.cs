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
    }
}

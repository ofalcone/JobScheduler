using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace JobScheduler.Data
{
    /// <summary>
    /// Crea gli utenti di default
    /// </summary>
    public class JobSchedulerDataSeed
    {
        private readonly JobSchedulerContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;


        

        public JobSchedulerDataSeed
            (
            JobSchedulerContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }


        public async Task SeedAsync()
        {
            var adminRole = await CreateAdminRole();
            var adminUser = await CreateAdminUser();
            await CreateAdminRole(adminRole, adminUser);
            await CreateTestJobs();
            await CreateTestNodes();
            await CreateTestGroups();
        }

        private async Task CreateAdminRole(IdentityRole adminRole, User adminUser)
        {
            if (adminRole == null || adminUser == null) return;

            await _userManager.AddToRoleAsync(adminUser, adminRole.Name);

            await UtilityDatabase.TryCommit<Node>(_context);
        }

        private async Task<IdentityRole> CreateAdminRole()
        {
            const string tipoRuolo = "Admin";

            var foundRole = await _roleManager.FindByNameAsync(tipoRuolo);

            if (foundRole == null)
            {
                foundRole = new IdentityRole
                {
                    Name = tipoRuolo
                };

                var result = await _roleManager.CreateAsync(foundRole);

                if (result.Succeeded == false)
                {
                    return null;
                }

                foundRole = await _roleManager.FindByNameAsync(tipoRuolo);

                foundRole = await UtilityDatabase.TryCommit<IdentityRole>(_context, foundRole);
            }

            return foundRole;
        }

        private async Task<User> CreateAdminUser()
        {
            string userName = _configuration["AdminUserInfo:User"];
            string password = _configuration["AdminUserInfo:Password"];
            string firstName = _configuration["AdminUserInfo:FirstName"];
            string lastname = _configuration["AdminUserInfo:LastName"];


        User user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = firstName,
                    LastName = lastname,
                };

                var result = await _userManager.CreateAsync(user, password);
                user = await _userManager.FindByEmailAsync(userName);

                if (!result.Succeeded)
                {
                    user = null;
                }
            }

            user = await UtilityDatabase.TryCommit<User>(_context,user);

            return user;
        }

        private async Task CreateTestJobs()
        {
            var result = _context.Jobs.Count();
            if (result < 1)
            {
                Job job1 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/chrome.exe"), Description = "launch chrome" };
                Job job2 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/firefox.exe"), Description = "launch firefox" };
                Job job3 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/edge.exe"), Description = "launch edge" };

                _context.Jobs.AddRange(job1, job2, job3);

                await UtilityDatabase.TryCommit<Job>(_context);
            }
        }

        private async Task CreateTestNodes()
        {
            var result = _context.Nodes.Count();
            if (result < 1)
            {
                Node node1 = new Node { Desc = "Node1" };
                Node node2 = new Node { Desc = "Node2" };
                Node node3 = new Node { Desc = "Node3" };

                _context.Nodes.AddRange(node1, node2, node3);

                await UtilityDatabase.TryCommit<Node>(_context);
            }
        }

        private async Task CreateTestGroups()
        {
            var result = _context.Groups.Count();
            if (result < 1)
            {
                Group group1 = new Group { Desc = "Group1" };
                Group group2 = new Group { Desc = "Group2" };

                _context.Groups.AddRange(group1, group2);

                await UtilityDatabase.TryCommit<Group>(_context);
            }
        }
    }

}

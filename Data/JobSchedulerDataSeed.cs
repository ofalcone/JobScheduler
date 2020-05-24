using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
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

        public JobSchedulerDataSeed
            (
            JobSchedulerContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
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

            await TryCommit<Node>();
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

                foundRole = await TryCommit<IdentityRole>(foundRole);
            }

            return foundRole;
        }

        private async Task<User> CreateAdminUser()
        {
            const string userName = "admin@jobscheduler.com";
            const string password = "Pippo92!";
            User user = await _userManager.FindByEmailAsync(userName);
            if (user == null)
            {
                user = new User
                {
                    UserName = userName,
                    Email = userName,
                    FirstName = "Admin",
                    LastName = "Admini"
                };

                var result = await _userManager.CreateAsync(user, password);
                user = await _userManager.FindByEmailAsync(userName);

                if (!result.Succeeded)
                {
                    user = null;
                }
            }

            user = await TryCommit<User>(user);

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

                await TryCommit<Job>();
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

                await TryCommit<Node>();
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

                await TryCommit<Group>();
            }
        }

        private async Task<T> TryCommit<T>(T obj = default)
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                obj = default;
            }

            return obj;
        }
    }

}

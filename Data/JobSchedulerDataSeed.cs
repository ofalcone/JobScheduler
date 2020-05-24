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
            var r = await CreateRole();
            var u = await CreateAdminUser();
            await CreateAdminRole(r, u);
            await CreateTestJobs();
            await CreateTestNodes();
            await CreateTestGroups();
            await _context.SaveChangesAsync();

        }

        private async Task CreateAdminRole(IdentityRole r, User u) 
        {
            var userRole = new IdentityUserRole<string>
            {
                UserId = u.Id,
                RoleId = r.Id
            };
            //dbContext.UserRoles.Add(userRole);
            await _userManager.AddToRoleAsync(u, r.Name);
        }

        private async Task<IdentityRole> CreateRole()
        {
            const string tipoRuolo = "Admin";

            var role = new IdentityRole
            {
                Name = tipoRuolo
            };
            var result = await _roleManager.CreateAsync(role);
            return  await _roleManager.FindByNameAsync(tipoRuolo);
             
        }

        private async Task<User> CreateAdminUser()
        {
            const string userName = "admin@jobscheduler.com";
            const string password = "Pippo92!";
            User u = new User(); 

            var user = await _userManager.FindByEmailAsync(userName);
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
                u = await _userManager.FindByEmailAsync(userName);
                

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Cannot create default user");
                }
            }
            return u;
        }

        private async Task CreateTestJobs()
        {
            var result = _context.Jobs.Count();
            if (result < 1)
            {
                Job job1 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/chrome.exe"), Description = "launch chrome" };
                Job job2 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/firefox.exe"), Description = "launch firefox" };
                Job job3 = new Job { Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/edge.exe"), Description = "launch edge" };


                //_context.Jobs.Add(job1);
                _context.Jobs.AddRange(job1, job2, job3);

                await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
            }
        }

        
    }

}

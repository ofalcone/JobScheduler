using JobScheduler.Models;
using Microsoft.AspNetCore.Identity;
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

       public JobSchedulerDataSeed(JobSchedulerContext context, UserManager<User> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task SeedAsync()
        {
            await CreateAdminUser();

            await CreateTestJob();
        }

        private async Task CreateTestJob()
        {
            var result = _context.Jobs.Count();
            if (result < 1)
            {
                Job job1 = new Job {Orario = "30 9 17 05 *", Path = Path.Combine("C:/temp/chrome.exe") };

                _context.Jobs.Add(job1);
                await _context.SaveChangesAsync();
            }
        }

        private async Task CreateAdminUser()
        {
            const string userName = "admin@jobscheduler.com";
            const string password = "Pippo92!";

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
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Cannot create default user");
                }
            }
        }
    }
}

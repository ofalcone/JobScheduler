using JobScheduler.Controllers.Api;
using JobScheduler.Data;
using JobScheduler.Interfaces;
using JobScheduler.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class ScheduleJob : CronJobService, IScheduleJob
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo, LaunchJob launchJob, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        : base(cronExpression, timeZoneInfo)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            StartAsync(CancellationToken.None, launchJob);
        }

        public override Task StartAsync(CancellationToken cancellationToken, LaunchJob launchJob)
        {
            return base.StartAsync(cancellationToken, launchJob);
        }

        public override async Task DoWork(CancellationToken cancellationToken, LaunchJob launchJob)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetRequiredService<JobSchedulerContext>();
                DbContextUtility dbContextUtility = new DbContextUtility(dbcontext, _configuration);
                await dbContextUtility.Launch(launchJob);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

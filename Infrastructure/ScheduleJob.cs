using JobScheduler.Controllers.Api;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class ScheduleJob : CronJobService
    {
        private int id;
        private string path;
        private readonly JobSchedulerContext _jobSchedulerContext;
        private readonly IConfiguration _configuration;

        public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo, JobSchedulerContext jobSchedulerContext, IConfiguration configuration)
       : base(cronExpression, timeZoneInfo)
        {
            _jobSchedulerContext = jobSchedulerContext;
            _configuration = configuration;
            StartAsync(CancellationToken.None);
        }

        //public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo, int id, string path) 
        //    : this(cronExpression, timeZoneInfo)
        //{
        //    this.id = id;
        //    this.path = path;
        //}
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override async Task DoWork(CancellationToken cancellationToken)
        {
            //return base.DoWork(cancellationToken);

            //Call web api launch job
            LaunchJob launchJob = new LaunchJob
            {
                Id = id,
                Path = path
            };

            //TODO: capire come lanciare il job usando dbContextUtility.Launch
            DbContextUtility dbContextUtility = new DbContextUtility(_jobSchedulerContext, _configuration);
            await dbContextUtility.Launch(launchJob);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}

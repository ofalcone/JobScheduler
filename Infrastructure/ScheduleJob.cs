using JobScheduler.Controllers.Api;
using JobScheduler.Data;
using JobScheduler.Interfaces;
using JobScheduler.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class ScheduleJob : CronJobService, IScheduleJob
    {
        private LaunchJob _launchJob;
        private readonly JobSchedulerContext _jobSchedulerContext;
        private readonly IConfiguration _configuration;

        public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo, JobSchedulerContext jobSchedulerContext, IConfiguration configuration, LaunchJob launchJob)
       : base(cronExpression, timeZoneInfo)
        {
            _jobSchedulerContext = jobSchedulerContext;
            _configuration = configuration;
            _launchJob = launchJob;
            StartAsync(CancellationToken.None, launchJob);
        }

        //public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo, int id, string path) 
        //    : this(cronExpression, timeZoneInfo)
        //{
        //    this.id = id;
        //    this.path = path;
        //}
        public override Task StartAsync(CancellationToken cancellationToken, LaunchJob launchJob)
        {
            return base.StartAsync(cancellationToken, launchJob);
        }

        public override async Task DoWork(CancellationToken cancellationToken, LaunchJob launchJob)
        {
            //return base.DoWork(cancellationToken);
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

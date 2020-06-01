﻿using JobScheduler.Controllers.Api;
using JobScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class ScheduleJob: CronJobService
    {
        private int id;
        private string path;

        public ScheduleJob(string cronExpression, TimeZoneInfo timeZoneInfo)
       : base(cronExpression, timeZoneInfo)
        {
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
                Path=path
            };
            await ApiJobsController.Launch(launchJob);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
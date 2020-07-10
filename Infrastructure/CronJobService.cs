using Cronos;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    /// <summary>
    /// Abstract class from which derive scheduled jobs to launch
    /// </summary>
    public abstract class CronJobService : IHostedService, IDisposable
    {
        //!= System.Threading.Timer
        private System.Timers.Timer _timer;
        private readonly CronExpression _expression;
        private readonly TimeZoneInfo _timeZoneInfo;

        protected CronJobService(string cronExpression, TimeZoneInfo timeZoneInfo)
        {
            _expression = CronExpression.Parse(cronExpression);
            _timeZoneInfo = timeZoneInfo;
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken)
        {
            //await ScheduleJob(cancellationToken);
        }

        public virtual async Task StartAsync(CancellationToken cancellationToken, Models.LaunchJob launchJob)
        {
            await ScheduleJob(cancellationToken, launchJob);
        }

        protected virtual async Task ScheduleJob(CancellationToken cancellationToken, Models.LaunchJob launchJob)
        {
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, _timeZoneInfo);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    _timer.Dispose();  // reset and dispose timer
                    _timer = null;

                    if (!cancellationToken.IsCancellationRequested)
                    {
                        //chiama il metodo ovverridato in ScheduleJob
                        await DoWork(cancellationToken, launchJob);
                    }

                    
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        //richiama se stesso per rischedulazioni
                        await ScheduleJob(cancellationToken, launchJob);    // reschedule next
                    }
                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        public virtual async Task DoWork(CancellationToken cancellationToken, Models.LaunchJob launchJob)
        {
            await Task.Delay(5000, cancellationToken);  // do the work
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Stop();
            await Task.CompletedTask;
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }
    }
}

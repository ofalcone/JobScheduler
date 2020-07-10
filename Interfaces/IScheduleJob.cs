using JobScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JobScheduler.Interfaces
{
    public interface IScheduleJob
    {
        Task DoWork(CancellationToken cancellationToken, LaunchJob launchJob);
    }
}

using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class NodeLaunchResultUtility
    {
        private readonly JobSchedulerContext _context;

        public NodeLaunchResultUtility(JobSchedulerContext context)
        {
            _context = context;
        }

        internal async Task<IEnumerable<NodeLaunchResult>> GetAll()
        {
            return await _context.NodesLaunchResults.ToArrayAsync();
        }
    }
}

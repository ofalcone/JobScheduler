using JobScheduler.Data;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Infrastructure
{
    public class JobGroupsUtility
    {

        private readonly JobSchedulerContext _context;

        public JobGroupsUtility(JobSchedulerContext context)
        {
            _context = context;
        }

        internal async Task<IEnumerable<JobGroup>> GetAll()
        {
            var jobSchedulerContext = _context.JobGroupes.Include(j => j.Group).Include(j => j.Job);
            var list = await jobSchedulerContext.ToArrayAsync();
            return list;
        }

        internal async Task<JobGroup> GetSingle(JobGroup jobGroup)
        {
            if (jobGroup == null)
            {
                return null;
            }

            return await _context.JobGroupes.FindAsync(jobGroup.JobId,jobGroup.GroupId);
        }

        internal async Task CreateSingle(JobGroup jobGroup)
        {
            await _context.JobGroupes.AddAsync(jobGroup);
            await _context.SaveChangesAsync();
        }

        internal async Task<bool> JobGroupExists(JobGroup jobGroup)
        {
            return await _context.JobGroupes.AnyAsync(e => e.JobId == jobGroup.JobId && e.GroupId == jobGroup.GroupId);
        }

        internal async Task Update(JobGroupViewModel jobGroupViewModel)
        {
            var oldJobGroup = new JobGroup
            {
                JobId = jobGroupViewModel.OldJobId,
                GroupId = jobGroupViewModel.OldGroupId
            };

            _context.JobGroupes.Remove(oldJobGroup);
            await _context.SaveChangesAsync();

            var newJobGroup = new JobGroup
            {
                JobId = jobGroupViewModel.NewJobId,
                GroupId = jobGroupViewModel.NewGroupId
            };

            bool alreadyExist = await JobGroupExists(newJobGroup);
            if (alreadyExist)
            {
                return;
            }

            _context.JobGroupes.Add(newJobGroup);
            await _context.SaveChangesAsync();
        }

        internal async Task Delete(JobGroup jobGroup)
        {
            _context.JobGroupes.Remove(jobGroup);
            await _context.SaveChangesAsync();
        }
    }
}

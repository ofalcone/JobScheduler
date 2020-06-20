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

            //bool exist = await _jobGroupsUtility.JobGroupExists(jobGroup);

            //jobGroup.Group = await _context.Groups.FindAsync(jobGroup.GroupId);
            //jobGroup.Job = await _context.Jobs.FindAsync(jobGroup.JobId);

            return await _context.JobGroupes.FindAsync(jobGroup);
        }


        internal async Task CreateSingle(JobGroup jobGroup)
        {
            _context.JobGroupes.Add(jobGroup);
            await _context.SaveChangesAsync();
        }

        internal async Task<bool> JobGroupExists(JobGroup jobGroup)
        {
            return await _context.JobGroupes.AnyAsync(e => e.JobId == jobGroup.JobId && e.GroupId == jobGroup.GroupId);
        }


        internal async Task Update(JobGroupViewModel jobGroupViewModel)
        {
            //TODO: far ritornare un result con info di success o failure
            var oldJobGroup = new JobGroup
            {
                JobId = jobGroupViewModel.OldJobId,
                GroupId = jobGroupViewModel.OldGroupId
            };

            _context.JobGroupes.Remove(oldJobGroup);

            var jobGroup = new JobGroup
            {
                JobId = jobGroupViewModel.NewJobId,
                GroupId = jobGroupViewModel.NewGroupId
            };

            _context.JobGroupes.Add(jobGroup);
            await _context.SaveChangesAsync();
        }

        internal async Task Delete(int oldJobId, int oldGroupId)
        {
            var jobGroup = new JobGroup
            {
                JobId = oldJobId,
                GroupId = oldGroupId
            };


            _context.JobGroupes.Remove(jobGroup);
            await _context.SaveChangesAsync();
        }
    }
}

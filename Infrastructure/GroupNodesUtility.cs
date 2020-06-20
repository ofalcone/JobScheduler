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
    public class GroupNodesUtility
    {

        private readonly JobSchedulerContext _context;

        public GroupNodesUtility(JobSchedulerContext context)
        {
            _context = context;
        }

        internal async Task<IEnumerable<GroupNode>> GetAll()
        {
            var jobSchedulerContext = _context.GroupNodes.Include(j => j.Group).Include(j => j.Node);
            var list = await jobSchedulerContext.ToArrayAsync();
            return list;
        }

        internal async Task<GroupNode> GetSingle(GroupNode groupNode)
        {
            if (groupNode == null)
            {
                return null;
            }

            //bool exist = await _jobGroupsUtility.JobGroupExists(groupNode);

            //groupNode.Group = await _context.Groups.FindAsync(groupNode.GroupId);
            //groupNode.Job = await _context.Jobs.FindAsync(groupNode.NodeId);

            return await _context.GroupNodes.FindAsync(groupNode.GroupId, groupNode.NodeId);
        }


        internal async Task CreateSingle(GroupNode groupNode)
        {
            await _context.GroupNodes.AddAsync(groupNode);
            await _context.SaveChangesAsync();
        }

        internal async Task<bool> GroupNodeExists(GroupNode groupNode)
        {
            return await _context.GroupNodes.AnyAsync(e => e.NodeId == groupNode.NodeId && e.GroupId == groupNode.GroupId);
        }

        internal async Task Update(GroupNodeViewModel groupNodeViewModel)
        {
            //TODO: far ritornare un result con info di success o failure
            var oldJobGroup = new GroupNode
            {
                NodeId = groupNodeViewModel.OldNodeId,
                GroupId = groupNodeViewModel.OldGroupId
            };

            _context.GroupNodes.Remove(oldJobGroup);
            await _context.SaveChangesAsync();

            var groupNodeNew = new GroupNode
            {
                NodeId = groupNodeViewModel.NewNodeId,
                GroupId = groupNodeViewModel.NewGroupId
            };

            var alreadyExist = await GroupNodeExists(groupNodeNew);
            if (alreadyExist)
            {
                return;
            }

            _context.GroupNodes.Add(groupNodeNew);
            await _context.SaveChangesAsync();
        }

        internal async Task Delete(GroupNode groupNode)
        {
            _context.GroupNodes.Remove(groupNode);
            await _context.SaveChangesAsync();
        }
    }

}

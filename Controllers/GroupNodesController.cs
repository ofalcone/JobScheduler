using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using JobScheduler.Infrastructure;
using JobScheduler.ViewModels;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = RolesNames.Admin)]
    public class GroupNodesController : Controller
    {
        private readonly JobSchedulerContext _context;
        private readonly GroupNodesUtility _groupsNodesUtility;

        public GroupNodesController(JobSchedulerContext context)
        {
            _context = context;
            _groupsNodesUtility = new GroupNodesUtility(context);
        }


        public async Task<IActionResult> Index()
        {
            return View(await _groupsNodesUtility.GetAll());
        }


        public async Task<IActionResult> Details(GroupNode groupNode)
        {
            bool groupNodeExist = await _groupsNodesUtility.GroupNodeExists(groupNode);
            if (!groupNodeExist)
            {
                return NotFound();
            }

            groupNode.Group = await _context.Groups.FindAsync(groupNode.GroupId);
            groupNode.Node = await _context.Nodes.FindAsync(groupNode.NodeId);

            return View(groupNode);
        }


        public IActionResult Create()
        {
            var n = _context.Nodes;
            var g = _context.Groups;

            ViewData["GroupId"] = new SelectList(g, "Id", "Desc");
            ViewData["NodeId"] = new SelectList(n, "Id", "Desc");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,NodeId")] GroupNode groupNode)
        {
            await _groupsNodesUtility.CreateSingle(groupNode);
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(GroupNode groupNode)
        {
            bool exist = await _groupsNodesUtility.GroupNodeExists(groupNode);

            if (!exist)
            {
                return NotFound();
            }

            GroupNodeViewModel groupNodeViewModel = new GroupNodeViewModel
            {
                OldNodeId = groupNode.NodeId,
                OldGroupId = groupNode.GroupId
            };

            ViewData["GroupId"] = new SelectList(_context.Groups, nameof(Group.Id), nameof(Group.Desc), groupNode.GroupId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, nameof(Node.Id), nameof(Node.Desc), groupNode.NodeId);
            
            return View(groupNodeViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GroupNodeViewModel groupNodeViewModel)
        {
            try
            {
                await _groupsNodesUtility.Update(groupNodeViewModel);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(GroupNode groupNode)
        {
            bool exist = await _groupsNodesUtility.GroupNodeExists(groupNode);

            if (!exist)
            {
                return NotFound();
            }

            groupNode.Group = await _context.Groups.FindAsync(groupNode.GroupId);
            groupNode.Node = await _context.Nodes.FindAsync(groupNode.NodeId);
            return View(groupNode);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("NodeId,GroupId")]GroupNode groupNode)
        {
            try
            {
                await _groupsNodesUtility.Delete(groupNode);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool GroupNodeExists(int id)
        {
            return _context.GroupNodes.Any(e => e.GroupId == id);
        }
    }
}

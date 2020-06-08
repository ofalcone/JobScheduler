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

namespace JobScheduler.Controllers
{
    [Authorize]
    public class GroupNodesController : Controller
    {
        private readonly JobSchedulerContext _context;

        public GroupNodesController(JobSchedulerContext context)
        {
            _context = context;
        }

        // GET: GroupNodes
        public async Task<IActionResult> Index()
        {
            var jobSchedulerContext = _context.GroupNodes.Include(g => g.Group).Include(g => g.Node);
            return View(await jobSchedulerContext.ToListAsync());
        }

        // GET: GroupNodes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupNode = await _context.GroupNodes
                .Include(g => g.Group)
                .Include(g => g.Node)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (groupNode == null)
            {
                return NotFound();
            }

            return View(groupNode);
        }

        // GET: GroupNodes/Create
        public IActionResult Create()
        {
            var n = _context.Nodes;
            var g = _context.Groups.Select(
                group => new
                {
                    Id = group.Id,
                    Desc = group.Id + group.Desc
                }
                ).ToArray();

            ViewData["GroupId"] = new SelectList(g, "Id", "Desc");
            //ViewData["NodeId"] = new SelectList(n, "Id", "Desc");
            return View();
        }

        // POST: GroupNodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GroupId,NodeId")] GroupNode groupNode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(groupNode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupNode.GroupId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", groupNode.NodeId);
            return View(groupNode);
        }

        // GET: GroupNodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupNode = await _context.GroupNodes
               .Include(j => j.Group)
               .Include(j => j.Node)
               .FirstOrDefaultAsync(m => m.GroupId == id); ;
            if (groupNode == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupNode.GroupId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", groupNode.NodeId);
            return View(groupNode);
        }

        // POST: GroupNodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GroupId,NodeId")] GroupNode groupNode)
        {
            if (id != groupNode.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupNode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupNodeExists(groupNode.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupNode.GroupId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", groupNode.NodeId);
            return View(groupNode);
        }

        // GET: GroupNodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupNode = await _context.GroupNodes
                .Include(g => g.Group)
                .Include(g => g.Node)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (groupNode == null)
            {
                return NotFound();
            }

            return View(groupNode);
        }

        // POST: GroupNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupNode = await _context.GroupNodes.FindAsync(id);
            _context.GroupNodes.Remove(groupNode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupNodeExists(int id)
        {
            return _context.GroupNodes.Any(e => e.GroupId == id);
        }
    }
}

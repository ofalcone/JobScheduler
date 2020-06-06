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
    public class JobGroupesController : Controller
    {
        private readonly JobSchedulerContext _context;

        public JobGroupesController(JobSchedulerContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var jobSchedulerContext = _context.JobGroupes.Include(j => j.Job).Include(j => j.Group);
            return View(await jobSchedulerContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobNode = await _context.JobGroupes
                .Include(j => j.Job)
                .Include(j => j.Group)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (jobNode == null)
            {
                return NotFound();
            }

            return View(jobNode);
        }

        public IActionResult Create()
        {
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id");
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(JobNode jobNode)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobNode);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.NodeId);
            return View(jobNode);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobNode = await _context.JobGroupes.FindAsync(id);
            if (jobNode == null)
            {
                return NotFound();
            }
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.GroupId);
            return View(jobNode);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,NodeId")] JobNode jobNode)
        {
            if (id != jobNode.JobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobNode);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobNodeExists(jobNode.JobId))
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
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
            ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.NodeId);
            return View(jobNode);
        }

        // GET: JobGroupes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobNode = await _context.JobGroupes
                .Include(j => j.Job)
                .Include(j => j.Group)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (jobNode == null)
            {
                return NotFound();
            }

            return View(jobNode);
        }

        // POST: JobGroupes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobNode = await _context.JobGroupes.FindAsync(id);
            _context.JobGroupes.Remove(jobNode);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobNodeExists(int id)
        {
            return _context.JobGroupes.Any(e => e.JobId == id);
        }
    }
}

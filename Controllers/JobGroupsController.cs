using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;

namespace JobScheduler.Controllers
{
    public class JobGroupsController : Controller
    {
        private readonly JobSchedulerContext _context;

        public JobGroupsController(JobSchedulerContext context)
        {
            _context = context;
        }

        // GET: JobGroups
        public async Task<IActionResult> Index()
        {
            var jobSchedulerContext = _context.JobGroupes.Include(j => j.Group).Include(j => j.Job);
            return View(await jobSchedulerContext.ToListAsync());
        }

        // GET: JobGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGroup = await _context.JobGroupes
                .Include(j => j.Group)
                .Include(j => j.Job)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (jobGroup == null)
            {
                return NotFound();
            }

            return View(jobGroup);
        }

        // GET: JobGroups/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id");
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id");
            return View();
        }

        // POST: JobGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,GroupId")] JobGroup jobGroup)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jobGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", jobGroup.GroupId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobGroup.JobId);
            return View(jobGroup);
        }

        // GET: JobGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGroup = await _context.JobGroupes.FindAsync(id);
            if (jobGroup == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", jobGroup.GroupId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobGroup.JobId);
            return View(jobGroup);
        }

        // POST: JobGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JobId,GroupId")] JobGroup jobGroup)
        {
            if (id != jobGroup.JobId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobGroupExists(jobGroup.JobId))
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
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", jobGroup.GroupId);
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobGroup.JobId);
            return View(jobGroup);
        }

        // GET: JobGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jobGroup = await _context.JobGroupes
                .Include(j => j.Group)
                .Include(j => j.Job)
                .FirstOrDefaultAsync(m => m.JobId == id);
            if (jobGroup == null)
            {
                return NotFound();
            }

            return View(jobGroup);
        }

        // POST: JobGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jobGroup = await _context.JobGroupes.FindAsync(id);
            _context.JobGroupes.Remove(jobGroup);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobGroupExists(int id)
        {
            return _context.JobGroupes.Any(e => e.JobId == id);
        }
    }
}

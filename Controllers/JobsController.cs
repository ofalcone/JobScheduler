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

namespace JobScheduler.Controllers
{
    [Authorize]
    public class JobsController : Controller
    {
        private readonly JobSchedulerContext _context;

        public JobsController(JobSchedulerContext context)
        {
            _context = context;
        }

        // GET: Jobs
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await UtilityController.CallWebApi<object,List<Job>>("ApiJobs",Infrastructure.HttpMethodsEnum.GET));
        }

        // GET: Jobs/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.GET_BY_ID, id);
            if (result == null)
            {
                return View(null);
            }
            return View(result);
        }

        // GET: Jobs/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Path,Orario")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Path,Orario")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.DELETE, id);

            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5 - Se nella view ho <input type="submit" value="Delete" -> ActionName("Delete")
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.DELETE_CONFIRMED, id);
            if (result == null)
            {
                return View(null);
            }

            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}

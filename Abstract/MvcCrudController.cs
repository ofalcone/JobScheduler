using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobScheduler.Abstract
{
    public abstract class MvcCrudController<TContext,TResource> : Controller
        where TContext : DbContext
        where TResource : class, IHasId
    {
        private GenericCrud<TContext, TResource> _genericCrud;
        private TContext _context;
        private IConfiguration _configuration;
        public MvcCrudController(TContext context, IConfiguration configuration=null)
        {
            _context = context;
            _configuration = configuration;
            _genericCrud = new GenericCrud<TContext, TResource>(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _genericCrud.GetAll());
        }

        // GET: Nodes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var resource = await _genericCrud.GetSingle(id);

            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }

        // GET: Nodes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Nodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TResource node)
        {
            if (ModelState.IsValid)
            {
                await _genericCrud.Create(node);

                ScheduleJob(node);

                return RedirectToAction(nameof(Index));
            }
            return View(node);
        }

        // GET: Nodes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var resource = await _genericCrud.GetSingle(id);
            if (resource == null)
            {
                return NotFound();
            }
            return View(resource);
        }

        // POST: Nodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TResource resource)
        {
            if (id != resource.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _genericCrud.Update(id, resource);

                ScheduleJob(resource);

                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

        // GET: Nodes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var res = await _genericCrud.GetSingle(id);

            if (res == null)
            {
                return NotFound();
            }
            return View(res);
        }

        // POST: Nodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _genericCrud.GetSingle(id);
            if (resource == null)
            {
                return NotFound();
            }
            await _genericCrud.Delete(id);
            return RedirectToAction(nameof(Index));
        }

        private void ScheduleJob(TResource node)
        {
            var job = node as Job;
            if (node != null && _configuration != null)
            {
                new ScheduleJob(job.Orario, TimeZoneInfo.Local, _context as JobSchedulerContext, _configuration);
            }
        }
    }
}

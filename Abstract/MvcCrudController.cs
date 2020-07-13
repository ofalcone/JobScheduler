using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Interfaces;
using JobScheduler.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        private readonly IServiceScopeFactory _scopeFactory;
 
        public MvcCrudController(TContext context, IServiceScopeFactory scopeFactory=null, IConfiguration configuration = null)
        {
            _context = context;
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _genericCrud = new GenericCrud<TContext, TResource>(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _genericCrud.GetAll());
        }

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TResource resource)
        {
            if (ModelState.IsValid)
            {
                await _genericCrud.Create(resource);

                ScheduleJob(resource);

                return RedirectToAction(nameof(Index));
            }
            return View(resource);
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TResource resource)
        {
            Node nodo = resource as Node;

            //Check to avoid modifications on Master node
            if (id != resource.Id || (nodo != null && nodo.Tipo == Enums.NodeType.Master))
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

        private void ScheduleJob(TResource resource)
        {
            var job = resource as Job;
            if (resource != null && _configuration != null)
            {
                if (string.IsNullOrWhiteSpace(job.Orario) || string.IsNullOrWhiteSpace(job.Path))
                {
                    return;
                }

                var launchJob = new LaunchJob()
                {
                    Id = job.Id,
                    Orario=job.Orario,
                    Path = job.Path,
                    Argomenti=job.Argomenti
                };

               new ScheduleJob(job.Orario, TimeZoneInfo.Local, launchJob, _scopeFactory, _configuration); ;
            }
        }
    }
}

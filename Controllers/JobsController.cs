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
using JobScheduler.Abstract;

namespace JobScheduler.Controllers
{
    [Authorize]
    public class JobsController : MvcCrudController<JobSchedulerContext,Job>
    {
        public JobsController(JobSchedulerContext context) : base(context)
        {
            //_context = context;
        }

        // GET: Jobs
        //[HttpGet]
        //public async Task<IActionResult> Index()
        //{
        //    GenericCrud<JobSchedulerContext, Job> genericCRUD = new GenericCrud<JobSchedulerContext, Job>(_context);
        //    var listJobs = await genericCRUD.GetAll();
        //    return View(listJobs);
        //    //return View(await UtilityController.CallWebApi<object,List<Job>>("ApiJobs",HttpMethodsEnum.GET));
        //}

        //// GET: Jobs/Details/5
        //[HttpGet]
        //public async Task<IActionResult> Details(int id)
        //{
        //    if (id==0)
        //    {
        //        return View(null);
        //    }
        //    //var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.GET_BY_ID, id);
        //    GenericCRUD<JobSchedulerContext, Job> genericCRUD = new GenericCRUD<JobSchedulerContext, Job>(_context);
        //    var result = await genericCRUD.GetSingle(id);

        //    if (result == null)
        //    {
        //        return View(null);
        //    }
        //    return View(result);
        //}

        //// GET: Jobs/Create
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Jobs/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<RedirectToActionResult> Create([Bind("Id,Path,Orario")] Job job)
        //{
        //    GenericCRUD<JobSchedulerContext, Job> genericCRUD = new GenericCRUD<JobSchedulerContext, Job>(_context);
        //    var result = await genericCRUD.Create(job);

        //    //var result = await UtilityController.CallWebApi<Job, object>("ApiJobs/PostJob", HttpMethodsEnum.POST, job);

        //    return RedirectToAction(nameof(Index));
        //}

        //// GET: Jobs/Edit/5
        //[HttpGet]
        //public async Task<IActionResult> Edit(int id)
        //{
        //    if (id == 0)
        //    {
        //        return NotFound();
        //    }

        //    GenericCRUD<JobSchedulerContext, Job> genericCRUD = new GenericCRUD<JobSchedulerContext, Job>(_context);
        //    var job = await genericCRUD.GetSingle(id);

        //    if (job == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(job);
        //}

        //// POST: Jobs/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Path,Orario")] Job job)
        //{
        //    if (id != job.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(job);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!JobExists(job.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(job);
        //}

        //// GET: Jobs/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.DELETE, id);

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var job = await _context.Jobs
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (job == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(job);
        //}

        // POST: Jobs/Delete/5 - Se nella view ho <input type="submit" value="Delete" -> ActionName("Delete")
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int? id)
        //{
        //    var result = await UtilityController.CallWebApi<int?, Job>("ApiJobs", HttpMethodsEnum.DELETE_CONFIRMED, id);
        //    if (result == null)
        //    {
        //        return View(null);
        //    }

        //    return RedirectToAction(nameof(Index));
        //}

        public async Task<IActionResult> Launch(LaunchJob launchJob)
        {
            if (launchJob == null)
            {
                return NotFound();
            }

            var result = await UtilityController.CallWebApi<LaunchJob, object>("ApiJobs/LaunchJob", HttpMethodsEnum.POST, launchJob);
            return RedirectToAction(nameof(Index));
        }

        //private bool JobExists(int id)
        //{
        //    return _context.Jobs.Any(e => e.Id == id);
        //}
    }
}

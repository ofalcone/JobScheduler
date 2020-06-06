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
    public class JobNodesController : Controller
    {
        //private readonly JobSchedulerContext _context;

        //public JobNodesController(JobSchedulerContext context)
        //{
        //    _context = context;
        //}

        //// GET: JobNodes
        //public async Task<IActionResult> Index()
        //{
        //    //var jobSchedulerContext = _context.JobNodes.Include(j => j.Job).Include(j => j.Node);
        //    //return View(await jobSchedulerContext.ToListAsync());
        //    return View(null);
        //}

        //// GET: JobNodes/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //var jobNode = await _context.JobNodes
        //    //    .Include(j => j.Job)
        //    //    .Include(j => j.Node)
        //    //    .FirstOrDefaultAsync(m => m.JobId == id);
        //    //if (jobNode == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //return View(jobNode);
        //    return View(null);
        //}

        //// GET: JobNodes/Create
        //public IActionResult Create()
        //{
        //    ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id");
        //    ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id");
        //    return View();
        //}

        //// POST: JobNodes/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("JobId,NodeId")] JobNode jobNode)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(jobNode);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
        //    ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.NodeId);
        //    return View(jobNode);
        //}

        //// GET: JobNodes/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //var jobNode = await _context.JobNodes.FindAsync(id);
        //    //if (jobNode == null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
        //    //ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.NodeId);
        //    //return View(jobNode);
        //    return View(null);
        //}

        //// POST: JobNodes/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("JobId,NodeId")] JobNode jobNode)
        //{
        //    if (id != jobNode.JobId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(jobNode);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!JobNodeExists(jobNode.JobId))
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
        //    ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Id", jobNode.JobId);
        //    ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", jobNode.NodeId);
        //    return View(jobNode);
        //}

        //// GET: JobNodes/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    //var jobNode = await _context.JobNodes
        //    //    .Include(j => j.Job)
        //    //    .Include(j => j.Node)
        //    //    .FirstOrDefaultAsync(m => m.JobId == id);
        //    //if (jobNode == null)
        //    //{
        //    //    return NotFound();
        //    //}

        //    //return View(jobNode);
        //    return View(null);
        //}

        //// POST: JobNodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    //var jobNode = await _context.JobNodes.FindAsync(id);
        //    //_context.JobNodes.Remove(jobNode);
        //    //await _context.SaveChangesAsync();
        //    //return RedirectToAction(nameof(Index));
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool JobNodeExists(int id)
        //{
        //    //return _context.JobNodes.Any(e => e.JobId == id);
        //    return false;
        //}
    }
}

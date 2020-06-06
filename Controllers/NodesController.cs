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
    public class NodesController : Controller
    {
        private readonly JobSchedulerContext _context;
        private readonly GenericCrud<JobSchedulerContext, Node> _genericCrud;
        public NodesController(JobSchedulerContext context)
        {
            _genericCrud = new GenericCrud<JobSchedulerContext, Node>(context);
        }

        //GET: Nodes
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
            var node = await _genericCrud.GetSingle(id);

            if (node == null)
            {
                return NotFound();
            }
            return View(node);
        }

        //    // GET: Nodes/Create
        public IActionResult Create()
        {
            return View();
        }

        //    // POST: Nodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tipo,Desc")] Node node)
        {
            if (ModelState.IsValid)
            {
                await _genericCrud.Create(node);
                return RedirectToAction(nameof(Index));
            }
            return View(node);
        }

        //    // GET: Nodes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var node = await _genericCrud.GetSingle(id);
            if (node == null)
            {
                return NotFound();
            }
            return View(node);
        }

        //    // POST: Nodes/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //    // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tipo,Desc")] Node node)
        {
            if (id != node.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _genericCrud.Update(id, node);
                return RedirectToAction(nameof(Index));
            }
            return View(node);
        }

        //    // GET: Nodes/Delete/5
        //    public async Task<IActionResult> Delete(int? id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var node = await _context.Nodes
        //            .FirstOrDefaultAsync(m => m.Id == id);
        //        if (node == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(node);
        //    }

        //    // POST: Nodes/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(int id)
        //    {
        //        var node = await _context.Nodes.FindAsync(id);
        //        _context.Nodes.Remove(node);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool NodeExists(int id)
        //    {
        //        return _context.Nodes.Any(e => e.Id == id);
        //    }
    }
}

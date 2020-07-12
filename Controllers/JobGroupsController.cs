using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JobScheduler.Data;
using JobScheduler.Models;
using JobScheduler.Abstract;
using JobScheduler.ViewModels;
using JobScheduler.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class JobGroupsController : Controller
    {
        private readonly JobSchedulerContext _context;
        private readonly JobGroupsUtility _jobGroupsUtility;

        public JobGroupsController(JobSchedulerContext context)
        {
            _context = context;
            _jobGroupsUtility = new JobGroupsUtility(context);
        }

        public async Task<IActionResult> Index()
        {
            return View(await _jobGroupsUtility.GetAll());
        }

        public async Task<IActionResult> Details(JobGroup jobGroup)
        {
            jobGroup.Group = await _context.Groups.FindAsync(jobGroup.GroupId);
            jobGroup.Job = await _context.Jobs.FindAsync(jobGroup.JobId);

            return View(jobGroup);
        }

        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Desc");
            ViewData["JobId"] = new SelectList(_context.Jobs, "Id", "Description");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("JobId,GroupId")] JobGroup jobGroup)
        {
            await _jobGroupsUtility.CreateSingle(jobGroup);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(JobGroup jobGroup)
        {
            bool exist = await _jobGroupsUtility.JobGroupExists(jobGroup);

            if (!exist)
            {
                return NotFound();
            }

            JobGroupViewModel jobGroupViewModel = new JobGroupViewModel
            {
                OldJobId = jobGroup.JobId,
                OldGroupId = jobGroup.GroupId
            };

            ViewData["JobId"] = new SelectList(_context.Jobs, nameof(Job.Id), nameof(Job.Description), jobGroup.JobId);
            ViewData["GroupId"] = new SelectList(_context.Groups, nameof(Group.Id), nameof(Group.Desc), jobGroup.GroupId);

            return View(jobGroupViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(JobGroupViewModel jobGroupViewModel)
        {
            try
            {
                await _jobGroupsUtility.Update(jobGroupViewModel);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> Delete(JobGroup jobGroup)
        {
            bool exist = await _jobGroupsUtility.JobGroupExists(jobGroup);

            if (!exist)
            {
                return NotFound();
            }

            //TODO: scegliere tra 1: a ogni chiamata portarsi dietro un oggetto JobGroup già riempito; 2- chiamare ogni volta il db
            jobGroup.Group = await _context.Groups.FindAsync(jobGroup.GroupId);
            jobGroup.Job = await _context.Jobs.FindAsync(jobGroup.JobId);
            return View(jobGroup);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([Bind("JobId,GroupId")]JobGroup jobGroup)
        {
            try
            {
                await _jobGroupsUtility.Delete(jobGroup);
            }
            catch
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

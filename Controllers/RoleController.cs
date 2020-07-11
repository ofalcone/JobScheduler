using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class RoleController : Controller
    {
        private readonly RolesUtility _roleUtility;
        public RoleController(RoleManager<IdentityRole> roleManager, JobSchedulerContext context)
        {
            _roleUtility = new RolesUtility(roleManager, context);
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(_roleUtility.GetRoles());
        }


        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var role = await _roleUtility.GetRoleById(id);

            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        public async Task<IActionResult> Create(IdentityRole role)
        {
            if (ModelState.IsValid)
            {
                string errorResult = await _roleUtility.Create(role);

                if (string.IsNullOrWhiteSpace(errorResult))
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Create");
        }


        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole user = await _roleUtility.GetRoleById(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction(nameof(Index));
        }


        // POST: Nodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            if (role == null)
            {
                return NotFound();
            }

            var result = await _roleUtility.Update(role);
            if (result == null || result.Succeeded == false)
            {
                return View(role);
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: Nodes/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    IdentityRole user = await _roleUtility.GetRoleById(id);

        //    if (user != null)
        //        return View(user);
        //    else
        //        return RedirectToAction(nameof(Index));
        //}


        //// POST: Nodes/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    if (string.IsNullOrWhiteSpace(id))
        //    {
        //        return NotFound();
        //    }

        //    var res = await _roleUtility.Delete(id);

        //    if (res == null)
        //    {
        //        return NotFound();
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

    }
}

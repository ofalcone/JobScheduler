using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers
{
    public class RoleController : Controller
    {
        private readonly RolesUtility _roleUtility;
        public RoleController(RoleManager<IdentityRole> roleManager,JobSchedulerContext context)
        {
            _roleUtility = new RolesUtility(roleManager, context);
        }

        //[HttpGet]
        //public async Task<ActionResult> Index()
        //{
        //    //var RolesList = _userManager.Roles;
        //    var roleList = await UtilityController.CallWebApi<object, List<IdentityRole>>("ApiRoles", HttpMethodsEnum.GET);
        //    return View(roleList);
        //}

        //[HttpPost]
        //public async Task<IActionResult> Create([Required] string name)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
        //        if (result.Succeeded)
        //            return RedirectToAction("Index");
        //        else
        //            Errors(result);
        //    }
        //    return View(name);
        //}


        //// DELETE : ROLE
        //public async Task<IActionResult> Delete(string id)
        //{
        //    IdentityRole role = await roleManager.FindByIdAsync(id.ToString());
        //    if (role != null)
        //    {
        //        IdentityResult result = await roleManager.DeleteAsync(role);
        //        if (result.Succeeded)
        //            return RedirectToAction("Index");
        //        else
        //            Errors(result);
        //    }
        //    else
        //        ModelState.AddModelError("", "No role found");
        //    return View("Index", roleManager.Roles);
        //}

        //// POST : UPDATE 
        //[HttpPost]
        //public async Task<IActionResult> Update(RoleViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        IdentityRole role = (IdentityRole)await roleManager.FindByIdAsync(model.Name);
        //        if (role != null)
        //            //update
        //            return RedirectToAction("Index");
        //        else
        //            return RedirectToAction("Create");
        //    }

        //    return RedirectToAction(nameof(Index));

        //}

        //private void Errors(IdentityResult result)
        //{
        //    foreach (IdentityError error in result.Errors)
        //        ModelState.AddModelError("", error.Description);
        //}
        [HttpGet]
        public ActionResult Index()
        {
            return View(_roleUtility.GetRoles());
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
            IdentityRole user = await _roleUtility.GetUserById(id);

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
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole user = await _roleUtility.GetUserById(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction(nameof(Index));
        }

        // POST: Nodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var res = await _roleUtility.Delete(id);

            if (res == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

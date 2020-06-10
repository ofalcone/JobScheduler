using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        public RoleController(RoleManager<IdentityRole> roleMgr)
        {
            roleManager = roleMgr;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //var usersList = _userManager.Users;
            var roleList = await UtilityController.CallWebApi<object, List<IdentityRole>>("ApiRoles", HttpMethodsEnum.GET);
            return View(roleList);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            return View(name);
        }


        // DELETE : ROLE
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id.ToString());
            if (role != null)
            {
                IdentityResult result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "No role found");
            return View("Index", roleManager.Roles);
        }

        // POST : UPDATE 
        [HttpPost]
        public async Task<IActionResult> Update(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = (IdentityRole)await roleManager.FindByIdAsync(model.Name);
                if (role != null)
                    //update
                    return RedirectToAction("Index");
                else
                    return RedirectToAction("Create");
            }

            return RedirectToAction(nameof(Index));

        }

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}

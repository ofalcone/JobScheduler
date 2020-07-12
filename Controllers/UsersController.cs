using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly UserUtility _userUtility;

        public UsersController(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userManager = userManager;
            _userUtility = new UserUtility(userManager, context);
        }


        [HttpGet]
        public ActionResult Index()
        {
            return View(_userUtility.GetUsers());
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }
            var user = await _userUtility.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userView)
        {
            if (ModelState.IsValid)
            {
                string errorResult = await _userUtility.Create(userView);

                if (string.IsNullOrWhiteSpace(errorResult))
                {
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Create");
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            UserViewModel user = await _userUtility.GetUserById(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel userViewModel)
        {
            if (userViewModel == null)
            {
                return NotFound();
            }

            var result = await _userUtility.Update(userViewModel);
            if (result == null || result.Succeeded == false)
            {
                return View(userViewModel);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            UserViewModel user = await _userUtility.GetUserById(id);

            if (user != null)
                return View(user);
            else
                return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var res = await _userUtility.Delete(id);

            if (res == null)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
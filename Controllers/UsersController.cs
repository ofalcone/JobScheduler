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

namespace JobScheduler.Controllers
{
    //TODO: fare la struttura delle action (Edit,Delete) esattamente come in MvcCrudController (Edit richiama la View, l'edit effettiva viene fatta dopo aver confermato; idem per Delete)
    //TODO: controllare cosa un admin può modificare, servono altri campi??
    //[Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        //private JobSchedulerContext _context;
        private readonly UserUtility _userUtility;
        public UsersController(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userManager = userManager;
            //_context = context;
            _userUtility = new UserUtility(userManager, context);
        }

        // GET: Users
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


        // POST: Users/Create
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


        // POST: Nodes/Edit/5
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


        // GET: Nodes/Delete/5
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


        // POST: Nodes/Delete/5
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
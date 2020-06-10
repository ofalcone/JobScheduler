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
        public ActionResult IndexAsync()
        {
            //var usersList = _userManager.Users;
            //var usersList = await UtilityController.CallWebApi<object,List<User>>("Users", HttpMethodsEnum.GET);
            return View(_userUtility.GetUsers());
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

        [HttpPost]
        public async Task<IActionResult> Update(string id)
        {
            User user = await _userUtility.GetUserById(id);
            
            if (user != null)
                return View(user);
            else
                return RedirectToAction("Index");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", _userManager.Users);
        }

        void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }
    }
}
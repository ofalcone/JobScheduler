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
    //[Authorize]
    public class UsersController : Controller
    {
        private UserManager<User> _userManager;
        private JobSchedulerContext _context;

        public UsersController(UserManager<User> userManager, JobSchedulerContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Users
        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            //var usersList = _userManager.Users;
            var usersList = await UtilityController.CallWebApi<object,List<User>>("Users", HttpMethodsEnum.GET);
            return View(usersList);
            
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
                User user = new User();

                user.UserName = userView.Email;
                user.Email = userView.Email;
                user.FirstName = userView.FirstName;
                user.LastName = userView.LastName;
                string password = userView.Password;


                var result =await _userManager.CreateAsync(user, password);
                if (result.Succeeded == false)
                {
                    ModelState.AddModelError("error",result.Errors.FirstOrDefault<IdentityError>().ToString());
                }
                await UtilityDatabase.TryCommit<User>(_context,user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Update(string id)
        {
            User user = await _userManager.FindByIdAsync(id);
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
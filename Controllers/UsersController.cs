using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers
{
    [Authorize]
    public class Users : Controller
    {
        private UserManager<User> _userManager;
        private JobSchedulerContext _context;

        public Users(UserManager<User> userManager, JobSchedulerContext context) 
        { 
            _userManager = userManager;
            _context = context;
        }

        // GET: UsersList
        [HttpGet]
        public ActionResult Index()
        {
            var usersList = _userManager.Users;
            return View(usersList);
        }

        // POST: UsersList/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task Create([Bind("Email,Password")] User users)
        {
            if (ModelState.IsValid)
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
            }
            //return View(users);
        }
    }
}
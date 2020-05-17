﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Controllers
{
    public class UsersListController : Controller
    {
        private UserManager<User> _userManager;
        private JobSchedulerContext _context;
        
        
        public UsersListController(UserManager<User> userManager, JobSchedulerContext context) 
        { 
            _userManager = userManager;
            _context = context;
        }

        // GET: UsersList
        [HttpGet]
        public ActionResult AllUsers()
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
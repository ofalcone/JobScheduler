﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobScheduler.Controllers
{
    //TODO: fare i metodi con userManager e userRole qui oppure in dbcontext
    public class UserRoleController : Controller
    {
        private readonly JobSchedulerContext _context;
        private readonly RolesUtility _roleUtility;
        private readonly UserUtility _userUtility;
        public UserRoleController(JobSchedulerContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userUtility = new UserUtility(userManager, context);
            _roleUtility = new RolesUtility(roleManager, context);
        }


        public async Task<ActionResult> Index()
        {
            //var users = _userUtility.GetUsers();
            //var roles = _roleUtility.GetRoles();
            List<UserRoleViewModel> userRoleViewModels = new List<UserRoleViewModel>();

            var userRoles = _context.UserRoles.ToArray();
            foreach (var item in userRoles)
            {
                UserRoleViewModel userRoleViewModel = new UserRoleViewModel
                {
                    UserId = item.UserId,
                    User = await _userUtility.GetUserById(item.UserId),
                    RoleId = item.RoleId,
                    Role = await _roleUtility.GetRoleById(item.RoleId)
                };

                userRoleViewModels.Add(userRoleViewModel);
            }
            return View(userRoleViewModels.ToArray());
        }


        public async Task<ActionResult> Details([Bind("userId", "roleId")]string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            var item = await _context.UserRoles.FindAsync(userId, roleId);
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel
            {
                UserId = item.UserId,
                User = await _userUtility.GetUserById(item.UserId),
                RoleId = item.RoleId,
                Role = await _roleUtility.GetRoleById(item.RoleId)
            };
            return View(userRoleViewModel);
        }

        // GET: UserRoleController/Create
        public IActionResult Create()
        {
            var users = _userUtility.GetUsers();
            var roles = _roleUtility.GetRoles();
            ViewData["UserId"] = new SelectList(users, "Id", "Email");
            ViewData["RoleId"] = new SelectList(roles, "Id", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Add(identityUserRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Edit([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            var item = await _context.UserRoles.FindAsync(userId, roleId);
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel
            {
                UserId = item.UserId,
                User = await _userUtility.GetUserById(item.UserId),
                RoleId = item.RoleId,
                Role = await _roleUtility.GetRoleById(item.RoleId)
            };


            if (userRoleViewModel == null)
            {
                return NotFound();
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", userRoleViewModel.UserId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", userRoleViewModel.RoleId);
            return View(userRoleViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditConfirmed([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }
            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Update(identityUserRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Delete([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            var item = await _context.UserRoles.FindAsync(userId, roleId);
            UserRoleViewModel userRoleViewModel = new UserRoleViewModel
            {
                UserId = item.UserId,
                User = await _userUtility.GetUserById(item.UserId),
                RoleId = item.RoleId,
                Role = await _roleUtility.GetRoleById(item.RoleId)
            };


            if (userRoleViewModel == null)
            {
                return NotFound();
            }
            return View(userRoleViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
            {
                UserId = userId,
                RoleId = roleId
            };

            _context.UserRoles.Remove(identityUserRole);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobScheduler.Data;
using JobScheduler.Infrastructure;
using JobScheduler.Models;
using JobScheduler.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobScheduler.Controllers
{
    [Authorize(Roles = Constants.ADMIN_ROLE)]
    public class UserRoleController : Controller
    {
        private readonly JobSchedulerContext _context;
        private readonly RolesUtility _roleUtility;
        private readonly UserUtility _userUtility;
        private readonly UserRoleUtility _userRoleUtility;

        public UserRoleController(JobSchedulerContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userUtility = new UserUtility(userManager, context);
            _roleUtility = new RolesUtility(roleManager, context);
            _userRoleUtility = new UserRoleUtility(_userUtility, _roleUtility, _context);
        }


        public async Task<ActionResult> Index()
        {
            List<UserRoleViewModel> userRoleViewModels = await _userRoleUtility.GetUserRoles();
            return View(userRoleViewModels.ToArray());
        }


        public async Task<ActionResult> Details([Bind("userId", "roleId")]string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            UserRoleViewModel userRoleViewModel = await _userRoleUtility.GetUserRole(userId, roleId);
            return View(userRoleViewModel);
        }


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

            await _userRoleUtility.CreateUserRole(userId, roleId);
            return RedirectToAction(nameof(Index));
        }

       
        public async Task<IActionResult> Edit([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            var userRoleViewModel= await _userRoleUtility.GetSingle(userId, roleId);

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
            await _userRoleUtility.Update(userId, roleId);
            return RedirectToAction(nameof(Index));
        }


        public async Task<ActionResult> Delete([Bind("UserId,RoleId")] string userId, string roleId)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleId))
            {
                return NotFound();
            }

            var userRoleViewModel = await _userRoleUtility.GetSingle(userId, roleId);


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

            await _userRoleUtility.DeleteUserRole(userId, roleId);

            return RedirectToAction(nameof(Index));
        }
     
    }
}

using System;
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


        // GET: UserRoleController
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
                    RoleId= item.RoleId,
                    Role = await _roleUtility.GetRoleById(item.RoleId)
                };

                userRoleViewModels.Add(userRoleViewModel);
            }
            return View(userRoleViewModels.ToArray());
        }

        // GET: UserRoleController/Details/5
        public async Task<ActionResult> Details([Bind("userId","roleId")]string userId, string roleId)
        {
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserRoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("UserId,RoleId")] string userId,string roleId)
        {
            if (ModelState.IsValid)
            {
                IdentityUserRole<string> identityUserRole = new IdentityUserRole<string>
                {
                    UserId = userId,
                    RoleId = roleId
                };

                _context.UserRoles.Add(identityUserRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "Id", groupNode.GroupId);
            //ViewData["NodeId"] = new SelectList(_context.Nodes, "Id", "Id", groupNode.NodeId);
            //return View(groupNode);
            return RedirectToAction(nameof(Index));
        }

        // GET: UserRoleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserRoleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserRoleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IMapper mapper;
        private readonly IOptions<UserRoles> config;

        public UsersController(IUsersService usersService,
            IMapper mapper,
            IOptions<UserRoles> config)
        {
            this.usersService = usersService;
            this.mapper = mapper;
            this.config = config;
        }

        public async Task<IActionResult> Index()
        {
            var users = await this.usersService.AllAsync();

            var currentUsername = this.User.Identity.Name;

            var viewUsers = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userRoles = this.usersService.GetRolesAsync(user);

                var viewUser = this.mapper.Map<UserViewModel>(user);

                viewUser.Role = string.Join(", ", userRoles);

                viewUsers.Add(viewUser);
            }

            var currentUser = viewUsers.FirstOrDefault(x => x.UserName == currentUsername);

            viewUsers.Remove(currentUser);

            return View(viewUsers);
        }

        public async Task<IActionResult> AddAdminRole(string id)
        {
            var user = this.usersService.GetUserById(id);

            var userRoles = this.usersService.GetRolesAsync(user);

            var adminRole = config.Value.AdminRole;

            if (!userRoles.Contains(adminRole))
            {
                await this.usersService.AddToRoleAsync(user, adminRole);
            }
            
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveAdminRole(string id)
        {
            var user = this.usersService.GetUserById(id);

            var userRoles = this.usersService.GetRolesAsync(user);

            if (userRoles.Contains(config.Value.AdminRole))
            {
                await this.usersService.RemoveFromRoleAsync(user, config.Value.AdminRole);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddTherapistRole(string id)
        {
            var user = this.usersService.GetUserById(id);

            var userRoles = this.usersService.GetRolesAsync(user);

            var therapistRole = config.Value.TherapistRole;

            if (!userRoles.Contains(therapistRole))
            {
                await this.usersService.AddToRoleAsync(user, therapistRole);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveTherapistRole(string id)
        {
            var user = this.usersService.GetUserById(id);

            var userRoles = this.usersService.GetRolesAsync(user);

            if (userRoles.Contains(config.Value.TherapistRole))
            {
                await this.usersService.RemoveFromRoleAsync(user, config.Value.TherapistRole);
            }

            return RedirectToAction("Index");
        }
    }
}
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TherapistsController : Controller
    {
        private readonly IUsersService usersService;
        private readonly IOptions<UserRoles> config;
        private readonly ITherapistsService therapistsService;
        private readonly IProfilesService profilesService;
        private readonly IMapper mapper;

        public TherapistsController(
            IUsersService usersService,
            IOptions<UserRoles> config,
            ITherapistsService therapistsService,
            IProfilesService profilesService,
            IMapper mapper)
        {
            this.usersService = usersService;
            this.config = config;
            this.therapistsService = therapistsService;
            this.profilesService = profilesService;
            this.mapper = mapper;
        }

        // GET: Admin/Therapists/Index_Admin
        public async Task<IActionResult> Index_Admin(string profile)
        {
            var therapists = await this.therapistsService.GetTherapistsByProfile(profile);

            var therapistsView = therapists.ToList()
                .Select(x => this.mapper.Map<TherapistViewModel>(x));

            ViewData["Profile"] = profile;

            return View(therapistsView);
        }

        // GET: Admin/Therapists/Edit
        public async Task<IActionResult> Edit(string id)
        {
            var therapist = await this.therapistsService.GetUserByIdAsync(id);

            var viewTherapist = this.mapper.Map<TherapistViewModel>(therapist);

            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            return View(viewTherapist);
        }

        // POST: Admin/Therapists/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TherapistViewModel therapistViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await this.therapistsService.GetUserByIdAsync(therapistViewModel.Id);

                var oldProfile = this.HttpContext.Request.Form["OldProfile"].ToString();

                if (oldProfile != therapistViewModel.Profile)
                {
                    this.therapistsService.RemoveProfileFromUser(user);

                    this.therapistsService.AddProfileToUser(user, therapistViewModel.Profile);
                }

                var therapists = await this.therapistsService.GetTherapistsByProfile(therapistViewModel.Profile);

                var therapistsView = therapists.ToList()
                    .Select(x => this.mapper.Map<TherapistViewModel>(x));

                var userRoles = this.usersService.GetRolesAsync(user);

                var therapistRole = config.Value.TherapistRole;

                if (!userRoles.Contains(therapistRole))
                {
                    await this.usersService.AddToRoleAsync(user, therapistRole);
                }

                @ViewData["Profile"] = therapistViewModel.Profile;

                return View("Index_Admin", therapistsView);
            }

            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            return View(therapistViewModel);
        }

        // GET: Admin/Therapists/Add
        public async Task<IActionResult> Add(string id)
        {
            var user = await this.therapistsService.GetUserByIdAsync(id);

            var viewUser = this.mapper.Map<TherapistViewModel>(user);

            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            ViewData["UserName"] = user.UserName;

            return View(viewUser);
        }

        // POST: Admin/Therapists/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(TherapistViewModel therapistViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = await this.therapistsService.GetUserByIdAsync(therapistViewModel.Id);

                this.therapistsService.AddProfileToUser(user, therapistViewModel.Profile);

                var therapists = await this.therapistsService.GetTherapistsByProfile(therapistViewModel.Profile);

                var therapistsView = therapists.ToList()
                    .Select(x => this.mapper.Map<TherapistViewModel>(x));

                var userRoles = this.usersService.GetRolesAsync(user);

                var therapistRole = config.Value.TherapistRole;

                if (!userRoles.Contains(therapistRole))
                {
                    await this.usersService.AddToRoleAsync(user, therapistRole);
                }

                return View("Index_Admin", therapistsView);
            }
            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            ViewData["UserName"] = therapistViewModel.UserName;

            return View(therapistViewModel);
        }

        // GET: Admin/Therapists/Remove
        public async Task<IActionResult> Remove(string id)
        {
            var user = await this.therapistsService.GetUserByIdAsync(id);

            var therapistView = this.mapper.Map<TherapistViewModel>(user);

            return View(therapistView);
        }

        // POST: Admin/Therapists/Remove
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string id, TherapistViewModel therapistView)
        {
            var user = await this.therapistsService.GetUserByIdAsync(id);

            var profile = user.Profile.Name;

            this.therapistsService.RemoveProfileFromUser(user);

            var therapists = await this.therapistsService.GetTherapistsByProfile(profile);

            var therapistsView = therapists.ToList()
                .Select(x => this.mapper.Map<TherapistViewModel>(x));

            var userRoles = this.usersService.GetRolesAsync(user);

            if (userRoles.Contains(config.Value.TherapistRole))
            {
                await this.usersService.RemoveFromRoleAsync(user, config.Value.TherapistRole);
            }

            return RedirectToAction("Index", "Users", therapistsView);
        }
    }
}
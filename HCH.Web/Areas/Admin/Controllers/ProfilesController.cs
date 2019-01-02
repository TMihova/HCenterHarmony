using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using HCH.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly IUsersService usersService;
        private readonly IOptions<UserRoles> configRoles;
        private readonly IMapper mapper;

        public ProfilesController(
            IProfilesService profilesService,
            IUsersService usersService,
            IOptions<UserRoles> configRoles,
            IMapper mapper)
        {
            this.profilesService = profilesService;
            this.usersService = usersService;
            this.configRoles = configRoles;
            this.mapper = mapper;
        }

        // GET: Profiles
        public async Task<IActionResult> Index_Admin()
        {
            var profiles = await this.profilesService.All();

            var viewProfiles = profiles.Select(x => this.mapper.Map<ProfileViewModel>(x));

            //var viewProfiles = profiles.Select(x => new ProfileViewModel
            //{
            //    Id = x.Id,
            //    Name = x.Name,
            //    Description = x.Description
            //});

            return View(viewProfiles);
        }

        // GET: Profiles/Details/5        
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await this.profilesService.GetProfileById(id);

            if (profile == null)
            {
                return NotFound();
            }

            var profileView = this.mapper.Map<ProfileViewModel>(profile);

            return View(profileView);
        }

        // GET: Profiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                var profile = this.mapper.Map<HCH.Models.Profile>(profileViewModel);

                //var profile = new Profile
                //{
                //    Name = profileViewModel.Name,
                //    Description = profileViewModel.Description
                //};

                await this.profilesService.AddProfileAsync(profile);
                return RedirectToAction(nameof(Index_Admin));
            }
            return View(profileViewModel);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await this.profilesService.GetProfileById(id);

            if (profile == null)
            {
                return NotFound();
            }

            var profileView = this.mapper.Map<ProfileViewModel>(profile);

            return View(profileView);
        }

        // POST: Profiles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProfileViewModel profile)
        {
            if (id != profile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await this.profilesService.UpdateProfileAsync(id, profile.Name, profile.Description);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.profilesService.ProfileExists(profile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index_Admin));
            }
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {return NotFound();
            }

            HCH.Models.Profile profile = await this.profilesService.GetProfileById(id);

            if (profile == null)
            {
                return NotFound();
            }

            var profileView = this.mapper.Map<ProfileViewModel>(profile);

            return View(profileView);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await this.profilesService.RemoveProfileAsync(id);
            
            return RedirectToAction(nameof(Index_Admin));
        }        
    }
}

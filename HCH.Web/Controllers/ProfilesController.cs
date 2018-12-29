using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using HCH.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace HCH.Web.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly IUsersService usersService;
        private readonly IOptions<UserRoles> configRoles;

        public ProfilesController(
            IProfilesService profilesService,
            IUsersService usersService,
            IOptions<UserRoles> configRoles)
        {
            this.profilesService = profilesService;
            this.usersService = usersService;
            this.configRoles = configRoles;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
            var profiles = await this.profilesService.All();

            var viewProfiles = profiles.Select(x => new ProfileViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });
            
            return View(viewProfiles);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index_Admin()
        {
            var profiles = await this.profilesService.All();

            var viewProfiles = profiles.Select(x => new ProfileViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });
            
            return View(viewProfiles);
        }

        // GET: Profiles/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await this.profilesService.GetProfileById(id);
                //await _context.Profiles.FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // GET: Profiles/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProfileViewModel profileViewModel)
        {
            if (ModelState.IsValid)
            {
                var profile = new Profile
                {
                    Name = profileViewModel.Name,
                    Description = profileViewModel.Description
                };

                await this.profilesService.AddAsync(profile);
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
                //_context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    //_context.Update(profile);
                    //await _context.SaveChangesAsync();
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Profile profile = await this.profilesService.GetProfileById(id);
                //await _context.Profiles.FirstOrDefaultAsync(m => m.Id == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
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

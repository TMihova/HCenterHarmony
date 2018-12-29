using System.Linq;
using System.Threading.Tasks;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TherapistsController : Controller
    {
        private readonly ITherapistsService therapistsService;
        private readonly IProfilesService profilesService;

        public TherapistsController(ITherapistsService therapistsService, IProfilesService profilesService)
        {
            this.therapistsService = therapistsService;
            this.profilesService = profilesService;
        }

        
        public async Task<IActionResult> Index_Admin(string profile)
        {
            var therapists = await this.therapistsService.GetTherapistsByProfile(profile);
            var therapistsView = therapists.ToList()
                .Select(x => new TherapistViewModel
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Profile = x.Profile.Name
                });

            ViewData["Profile"] = profile;

            return View(therapistsView);
        }

        
        public async Task<IActionResult> Edit(string id)
        {
            var therapist = this.therapistsService.GetUserById(id);

            var viewTherapist = new TherapistViewModel
            {
                Id = therapist.Id,
                FullName = therapist.FirstName + " " + therapist.LastName,
                Profile = therapist.Profile?.Name,
                UserName = therapist.UserName
            };

            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            return View(viewTherapist);
        }

        [HttpPost]
        public IActionResult Edit(TherapistViewModel therapistViewModel)
        {


            return View();
        }

        
        public async Task<IActionResult> Add(string id)
        {
            var user = this.therapistsService.GetUserById(id);

            var viewUser = new TherapistViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FirstName + " " + user.LastName
            };

            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            ViewData["UserName"] = user.UserName;

            return View(viewUser);
        }

        [HttpPost]
        public async Task<IActionResult> Add(TherapistViewModel therapistViewModel)
        {

            if (ModelState.IsValid)
            {
                var user = this.therapistsService.GetUserById(therapistViewModel.Id);
                this.therapistsService.AddProfileToUser(user, therapistViewModel.Profile);

                var therapists = await this.therapistsService.GetTherapistsByProfile(therapistViewModel.Profile);
                var therapistsView = therapists.ToList()
                    .Select(x => new TherapistViewModel
                    {
                        Id = x.Id,
                        FullName = x.FirstName + " " + x.LastName,
                        Profile = x.Profile.Name
                    });

                return View("Index", therapistsView);
                //return RedirectToAction("Index", "Therapists", profile);
            }
            var profiles = await this.profilesService.All();

            var profileNames = profiles.Select(x => x.Name);

            ViewData["Profile"] = new SelectList(profileNames);

            ViewData["UserName"] = therapistViewModel.UserName;

            return View(therapistViewModel);
        }
        
        public async Task<IActionResult> Remove(string id)
        {
            var user = this.therapistsService.GetUserById(id);

            var profile = user.Profile.Name;

            this.therapistsService.RemoveProfileFromUser(user);

            var therapists = await this.therapistsService.GetTherapistsByProfile(profile);
            var therapistsView = therapists.ToList()
                .Select(x => new TherapistViewModel
                {
                    Id = x.Id,
                    FullName = x.FirstName + " " + x.LastName,
                    Profile = x.Profile.Name
                });

            return View("Index_Admin", therapistsView);
        }
    }
}
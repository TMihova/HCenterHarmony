using System.Linq;
using System.Threading.Tasks;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HCH.Web.Controllers
{
    public class TherapistsController : Controller
    {
        private readonly ITherapistsService therapistsService;
        private readonly IProfilesService profilesService;

        public TherapistsController(ITherapistsService therapistsService, IProfilesService profilesService)
        {
            this.therapistsService = therapistsService;
            this.profilesService = profilesService;
        }

        public async Task<IActionResult> Index(string profile)
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

    }
}
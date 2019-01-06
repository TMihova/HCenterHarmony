using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace HCH.Web.Controllers
{
    public class TherapistsController : Controller
    {
        private readonly ITherapistsService therapistsService;
        private readonly IProfilesService profilesService;
        private readonly IMapper mapper;

        public TherapistsController(
            ITherapistsService therapistsService,
            IProfilesService profilesService,
            IMapper mapper)
        {
            this.therapistsService = therapistsService;
            this.profilesService = profilesService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index(string profile)
        {
            var therapists = await this.therapistsService.GetTherapistsByProfile(profile);
            var therapistsView = therapists.ToList()
                .Select(x => this.mapper.Map<TherapistViewModel>(x));

            ViewData["Profile"] = profile;

            return View(therapistsView);
        }

    }
}
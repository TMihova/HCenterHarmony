using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Web.Models;
using HCH.Services;
using Microsoft.Extensions.Options;
using AutoMapper;

namespace HCH.Web.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            var profiles = await this.profilesService.All();

            var viewProfiles = profiles.Select(x => this.mapper.Map<ProfileViewModel>(x));

            foreach (var item in viewProfiles)
            {
                if (item.Description.Length > 100)
                {
                    item.Description = item.Description.Substring(0, 100);
                }
                
            }

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

            var profileView = this.mapper.Map<ProfileDetailsViewModel>(profile);

            return View(profileView);
        }
    }
}

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
    }
}

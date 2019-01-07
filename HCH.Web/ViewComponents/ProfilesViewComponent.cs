using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Web.ViewComponents
{
    public class ProfilesViewComponent : ViewComponent
    {
        private readonly IProfilesService profilesService;
        private readonly IMapper mapper;

        public ProfilesViewComponent(
            IProfilesService profilesService,
            IMapper mapper)
        {
            this.profilesService = profilesService;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var profiles = await this.profilesService.All();

            var viewProfiles = profiles.Select(x => this.mapper.Map<ProfileViewModel>(x));

            return View(viewProfiles);
        }


    }
}

using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HCH.Web.ViewComponents
{
    public class TherapyViewComponent : ViewComponent
    {
        private readonly ITherapiesService therapiesService;
        private readonly IMapper mapper;

        public TherapyViewComponent(
            ITherapiesService therapiesService,
            IMapper mapper)
        {
            this.therapiesService = therapiesService;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {            
            var therapy = await this.therapiesService.GetTherapyByIdAsync(id);            

            var therapyModel = this.mapper.Map<TherapyViewModel>(therapy);

            return View(therapyModel);
        }
    }
}

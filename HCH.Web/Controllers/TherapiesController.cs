using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using HCH.Web.Models;

namespace HCH.Web.Controllers
{
    public class TherapiesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly ITreatmentsService treatmentsService;
        private readonly ITherapiesService therapiesService;
        private readonly IUsersService usersService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public TherapiesController(
            IProfilesService profilesService,
            ITreatmentsService treatmentsService,
            ITherapiesService therapiesService,
            IUsersService usersService,
            SignInManager<HCHWebUser> signInManager,
            IMapper mapper)
        {
            this.profilesService = profilesService;
            this.treatmentsService = treatmentsService;
            this.therapiesService = therapiesService;
            this.usersService = usersService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }
        
        // GET: Therapies/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapy = await this.therapiesService.GetTherapyByIdAsync(id);
                
            if (therapy == null)
            {
                return NotFound();
            }

            var therapyModel = this.mapper.Map<TherapyViewModel>(therapy);

            return View(therapyModel);
        }

        // GET: Therapies/Edit/5
        [Authorize(Roles ="Therapist")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var therapy = await this.therapiesService.GetTherapyByIdAsync(id);

            if (therapy == null)
            {
                return NotFound();
            }

            var therapyModel = this.mapper.Map<TherapyViewModel>(therapy);

            var therapistId = therapyModel.TherapistId;

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var treatmentsFromProfile = await this.treatmentsService.AllFromProfileAsync(therapist.ProfileId);

            var treatmentsView = treatmentsFromProfile.Select(x => this.mapper.Map<TherapyTreatmentViewModel>(x)).ToList();

            foreach (var item in treatmentsView)
            {
                if (therapyModel.Treatments.Contains(item))
                {
                    item.Selected = true;
                }
            }

            therapyModel.Treatments = treatmentsView;

            return View(therapyModel);
        }

        // POST: Therapies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,TherapyViewModel therapyView)
        {
            if (id != therapyView.TherapyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var therapy = this.mapper.Map<Therapy>(therapyView);

                    await this.therapiesService.UpdateTherapy(therapy);
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.therapiesService.TherapyExists(therapyView.TherapyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Examinations");
            }

            return View(therapyView);
        }
    }
}

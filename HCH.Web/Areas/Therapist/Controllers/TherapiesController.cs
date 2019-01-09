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
using System.Globalization;

namespace HCH.Web.Areas.Therapist.Controllers
{
    [Area("Therapist")]
    [Authorize(Roles = "Therapist")]
    public class TherapiesController : Controller
    {
        private readonly IProfilesService profilesService;
        private readonly ITreatmentsService treatmentsService;
        private readonly ITherapiesService therapiesService;
        private readonly IExaminationsService examinationsService;
        private readonly IUsersService usersService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public TherapiesController(
            IProfilesService profilesService,
            ITreatmentsService treatmentsService,
            ITherapiesService therapiesService,
            IExaminationsService examinationsService,
            IUsersService usersService,
            SignInManager<HCHWebUser> signInManager,
            IMapper mapper)
        {
            this.profilesService = profilesService;
            this.treatmentsService = treatmentsService;
            this.therapiesService = therapiesService;
            this.examinationsService = examinationsService;
            this.usersService = usersService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        // GET: Therapist/Therapies/Edit/5
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

        // POST: Therapist/Therapies/Edit/5
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
                if (therapyView.Treatments.All(x => x.Selected == false ))
                {
                    return View(therapyView);
                }

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

        // GET: Therapist/Therapies/Create/5
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examinationId = id.GetValueOrDefault();

            var examination = await this.examinationsService.GetExaminationByIdAsync(examinationId);

            if (examination == null)
            {
                return NotFound();
            }

            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var therapyModel = this.mapper.Map<TherapyViewModel>(examination);

            var treatmentsFromProfile = await this.treatmentsService.AllFromProfileAsync(therapist.ProfileId);

            var treatmentsView = treatmentsFromProfile.Select(x => this.mapper.Map<TherapyTreatmentViewModel>(x)).ToList();
            
            therapyModel.Treatments = treatmentsView;

            ViewData["ExaminationId"] = examination.Id;
            ViewData["ExaminationDate"] = examination.ExaminationDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);

            return View(therapyModel);
        }

        // POST: Therapist/Therapies/Create/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, TherapyViewModel therapyView)
        {

            if (ModelState.IsValid)
            {
                if (therapyView.Treatments.Count == 0)
                {
                    return View(therapyView);
                }

                if (therapyView.StartDate > therapyView.EndDate)
                {
                    this.ModelState.AddModelError("TherapyStartDate", "Невалидна дата за начало или край на    терапията.");

                    var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                    return this.View("Error", errors);
                }

                try
                {
                    var therapy = this.mapper.Map<Therapy>(therapyView);

                    await this.therapiesService.UpdateTherapy(therapy);

                    var examination = await this.examinationsService.GetExaminationByIdAsync(id);

                    examination.Therapy = therapy;

                    await this.examinationsService.UpdateExaminationAsync(examination);
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

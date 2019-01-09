using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using HCH.Web.Models;
using AutoMapper;
using X.PagedList;

namespace HCH.Web.Areas.Therapist.Controllers
{
    [Area("Therapist")]
    [Authorize(Roles = "Therapist")]
    public class TreatmentsController : Controller
    {
        private readonly ITreatmentsService treatmentsService;
        private readonly IProfilesService profilesService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public TreatmentsController(
            ITreatmentsService treatmentsService,
            IProfilesService profilesService,
            SignInManager<HCHWebUser> signInManager,
            IMapper mapper)
        {
            this.treatmentsService = treatmentsService;
            this.profilesService = profilesService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }
        
        // GET: Therapist/Treatments/Index_Therapist
        public async Task<IActionResult> Index_Therapist(int? page)
        {
            var therapist = await this.signInManager.UserManager.GetUserAsync(User);

            var profileId = therapist.ProfileId;            

            var treatments = await this.treatmentsService.AllFromProfileAsync(profileId);

            var treatmentsView = treatments
                .Select(x => this.mapper.Map<TreatmentViewModel>(x));

            var profile = await this.profilesService.GetProfileById(profileId);

            var numberOfItems = 8;

            var pageNumber = page ?? 1;

            var onePageOfTreatments = treatmentsView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            ViewData["ProfileName"] = profile.Name;

            return View(onePageOfTreatments);
        }

        // GET: Therapist/Treatments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await this.treatmentsService.GetTreatmentById(id);

            if (treatment == null)
            {
                return NotFound();
            }

            var treatmentView = this.mapper.Map<TreatmentViewModel>(treatment);

            return View(treatmentView);
        }

        // GET: Therapist/Treatments/Create
        [Authorize(Roles = "Therapist")]
        public IActionResult Create()
        {
            var therapist = this.signInManager.UserManager.GetUserAsync(User).Result;

            ViewData["ProfileId"] = therapist.ProfileId;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View();
        }

        // POST: Therapist/Treatments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TreatmentViewModel treatmentView)
        {
            if (ModelState.IsValid)
            {
                Treatment treatment = this.mapper.Map<Treatment>(treatmentView);

                await this.treatmentsService.AddTreatmentAsync(treatment);
                
                return RedirectToAction(nameof(Index_Therapist));
            }

            var therapist = this.signInManager.UserManager.GetUserAsync(User).Result;

            ViewData["ProfileId"] = therapist.ProfileId;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(treatmentView);
        }

        // GET: Therapist/Treatments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await this.treatmentsService.GetTreatmentById(id);

            if (treatment == null)
            {
                return NotFound();
            }

            var treatmentView = this.mapper.Map<TreatmentViewModel>(treatment);

            ViewData["ProfileId"] = treatment.ProfileId;

            var profile = await this.profilesService.GetProfileById(treatment.ProfileId);

            ViewData["ProfileName"] = profile.Name;

            return View(treatmentView);
        }

        // POST: Therapist/Treatments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, TreatmentViewModel treatmentView)
        {
            if (id != treatmentView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var treatment = this.mapper.Map<Treatment>(treatmentView);

                try
                {
                    await this.treatmentsService.UpdateTreatmentAsync(treatment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatmentView.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index_Therapist));
            }
            ViewData["ProfileId"] = treatmentView.ProfileId;

            var profile = await this.profilesService.GetProfileById(treatmentView.ProfileId);

            ViewData["ProfileName"] = profile.Name;

            return View(treatmentView);
        }

        

        // GET: Therapist/Treatments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await this.treatmentsService.GetTreatmentById(id);

            if (treatment == null)
            {
                return NotFound();
            }

            var treatmentView = this.mapper.Map<TreatmentViewModel>(treatment);

            return View(treatmentView);
        }

        // POST: Therapist/Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var treatment = await this.treatmentsService.GetTreatmentById(id);

            await this.treatmentsService.RemoveTreatmentAsync(treatment);
            
            return RedirectToAction(nameof(Index_Therapist));
        }

        private bool TreatmentExists(string id)
        {
            return this.treatmentsService.TreatementExists(id);                
        }
    }
}

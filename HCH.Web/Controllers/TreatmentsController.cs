﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using HCH.Web.Models;

namespace HCH.Web.Controllers
{
    public class TreatmentsController : Controller
    {
        private readonly ITreatmentsService treatmentsService;
        private readonly IProfilesService profilesService;
        private readonly SignInManager<HCHWebUser> signInManager;

        public TreatmentsController(
            ITreatmentsService treatmentsService,
            IProfilesService profilesService,
            SignInManager<HCHWebUser> signInManager)
        {
            this.treatmentsService = treatmentsService;
            this.profilesService = profilesService;
            this.signInManager = signInManager;
        }
        
        // GET: Treatments/Index_Therapist
        [Authorize(Roles="Therapist")]
        public async Task<IActionResult> Index_Therapist()
        {
            var therapist = await this.signInManager.UserManager.GetUserAsync(User);

            var profileId = therapist.ProfileId;            

            var treatments = await this.treatmentsService.AllFromProfileAsync(profileId);

            var treatmentsView = treatments
                .Select(x => new TreatmentViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price
                });

            var profile = await this.profilesService.GetProfileById(profileId);

            ViewData["ProfileName"] = profile.Name;

            return View(treatmentsView);
        }

        // GET: Treatments/Details/5
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

            var treatmentView = new TreatmentViewModel
            {
                Id = treatment.Id,
                Name = treatment.Name,
                Description = treatment.Description,
                Price = treatment.Price,
                ProfileId = treatment.ProfileId,
                Profile = treatment.Profile.Name
            };

            return View(treatmentView);
        }

        // GET: Treatments/Create
        [Authorize(Roles = "Therapist")]
        public IActionResult Create()
        {
            var therapist = this.signInManager.UserManager.GetUserAsync(User).Result;

            ViewData["ProfileId"] = therapist.ProfileId;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View();
        }

        // POST: Treatments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Create(TreatmentViewModel treatmentView)
        {
            if (ModelState.IsValid)
            {
                Treatment treatment = new Treatment
                {
                    Name = treatmentView.Name,
                    Description = treatmentView.Description,
                    Price = treatmentView.Price,
                    ProfileId = treatmentView.ProfileId
                };

                await this.treatmentsService.AddTreatmentAsync(treatment);
                
                return RedirectToAction(nameof(Index_Therapist));
            }

            var therapist = this.signInManager.UserManager.GetUserAsync(User).Result;

            ViewData["ProfileId"] = therapist.ProfileId;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(treatmentView);
        }

        // GET: Treatments/Edit/5
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await this.treatmentsService.GetTreatmentById(id);
                //await _context.Treatments.FindAsync(id);
            if (treatment == null)
            {
                return NotFound();
            }

            var treatmentView = new TreatmentViewModel
            {
                Id = treatment.Id,
                Name = treatment.Name,
                Description = treatment.Description,
                Price = treatment.Price,
                ProfileId = treatment.ProfileId,
                Profile = treatment.Profile.Name
            };

            ViewData["ProfileId"] = treatment.ProfileId;

            var profile = await this.profilesService.GetProfileById(treatment.ProfileId);

            ViewData["ProfileName"] = profile.Name;

            return View(treatmentView);
        }

        // POST: Treatments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Edit(string id, TreatmentViewModel treatmentView)
        {
            if (id != treatmentView.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var treatment = new Treatment
                {
                    Id = treatmentView.Id,
                    Name = treatmentView.Name,
                    Description = treatmentView.Description,
                    Price = treatmentView.Price,
                    ProfileId = treatmentView.ProfileId
                };

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

            var profile = await this.profilesService.GetProfileById(id);

            ViewData["ProfileName"] = profile.Name;

            return View(treatmentView);
        }

        

        // GET: Treatments/Delete/5
        [Authorize(Roles = "Therapist")]
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

            var treatmentView = new TreatmentViewModel
            {
                Id = treatment.Id,
                Name = treatment.Name,
                Description = treatment.Description,
                Price = treatment.Price,
                ProfileId = treatment.ProfileId,
                Profile = treatment.Profile.Name
            };

            return View(treatmentView);
        }

        // POST: Treatments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
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

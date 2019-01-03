using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCH.Data;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;

namespace HCH.Web.Controllers
{
    public class ExaminationsController : Controller
    {
        private readonly HCHWebContext _context;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly ITreatmentsService treatmentsService;
        private readonly ITherapiesService therapiesService;
        private readonly IExaminationsService examinationsService;
        private readonly IUsersService usersService;

        public ExaminationsController(HCHWebContext context,
            SignInManager<HCHWebUser> signInManager,
            ITreatmentsService treatmentsService,
            ITherapiesService therapiesService,
            IExaminationsService examinationsService,
            IUsersService usersService)
        {
            _context = context;
            this.signInManager = signInManager;
            this.treatmentsService = treatmentsService;
            this.therapiesService = therapiesService;
            this.examinationsService = examinationsService;
            this.usersService = usersService;
        }

        // GET: Examinations
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.Examinations.Include(e => e.Patient).Include(e => e.Therapist).Include(e => e.Therapy);
            return View(await hCHWebContext.ToListAsync());
        }

        // GET: Examinations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Therapist)
                .Include(e => e.Therapy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        // GET: Examinations/Create/patientId
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Create(string id)
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            HCHWebUser patient = this.usersService.GetUserById(id);

            var treatmentsFromProfile = await this.treatmentsService.AllFromProfileAsync(therapist.ProfileId);

            ExaminationViewModel examinationViewModel = new ExaminationViewModel
            {
                ExaminationDate = DateTime.UtcNow,
                PatientId = id,
                Patient = patient.FirstName + " " + patient.LastName,
                TherapistId = therapistId,
                Therapist = therapist.FirstName + " " + therapist.LastName,
                Anamnesis = "Обективно състояние: ",
                Treatments = treatmentsFromProfile.Select(x => new TherapyTreatmentViewModel
                {
                    TreatmentId = x.Id,
                    Name = x.Name,
                    Price = x.Price
                }).ToList(),
                TherapyStartDate = DateTime.UtcNow
            };

            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(examinationViewModel);
        }

        // POST: Examinations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExaminationViewModel examinationViewModel)
        {
            if (ModelState.IsValid)
            {
                if (examinationViewModel.TherapyStartDate < examinationViewModel.ExaminationDate)
                {
                    this.ModelState.AddModelError("TherapyStartDate", "Невалидна дата за начало на терапията.");

                    var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                    return this.View("Error", errors);
                }

                Therapy therapy = null;

                if (examinationViewModel.Treatments.Any(t => t.Selected == true))
                {
                    therapy = new Therapy
                    {
                        PatientId = examinationViewModel.PatientId,
                        TherapistId = examinationViewModel.TherapistId,
                        Treatments = examinationViewModel
                                      .Treatments.Where(t => t.Selected == true)
                                      .Select(t => new TherapyTreatment
                                      {
                                          TreatmentId = t.TreatmentId
                                      }).ToList(),
                        StartDate = examinationViewModel.TherapyStartDate,
                        EndDate = examinationViewModel.TherapyStartDate.AddDays(examinationViewModel.TherapyDuration)
                    };

                    await this.therapiesService.AddTherapyAsync(therapy);
                }

                var examination = new Examination
                {
                    ExaminationDate = examinationViewModel.ExaminationDate,
                    Anamnesis = examinationViewModel.Anamnesis,
                    PatientId = examinationViewModel.PatientId,
                    TherapistId = examinationViewModel.TherapistId,
                    Therapy = therapy
                };

                await this.examinationsService.AddExaminationAsync(examination);

                return RedirectToAction("Index", "Examinations");
            }

            var therapistId = this.signInManager.UserManager.GetUserId(User);
            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(examinationViewModel);
        }

        // GET: Examinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations.FindAsync(id);
            if (examination == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", examination.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", examination.TherapistId);
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id", examination.TherapyId);
            return View(examination);
        }

        // POST: Examinations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExaminationDate,TherapistId,PatientId,Anamnesis,TherapyId")] Examination examination)
        {
            if (id != examination.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examination);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExaminationExists(examination.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", examination.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", examination.TherapistId);
            ViewData["TherapyId"] = new SelectList(_context.Therapies, "Id", "Id", examination.TherapyId);
            return View(examination);
        }

        // GET: Examinations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examination = await _context.Examinations
                .Include(e => e.Patient)
                .Include(e => e.Therapist)
                .Include(e => e.Therapy)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (examination == null)
            {
                return NotFound();
            }

            return View(examination);
        }

        // POST: Examinations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var examination = await _context.Examinations.FindAsync(id);
            _context.Examinations.Remove(examination);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExaminationExists(int id)
        {
            return _context.Examinations.Any(e => e.Id == id);
        }
    }
}

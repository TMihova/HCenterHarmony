using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using AutoMapper;
using X.PagedList;

namespace HCH.Web.Areas.Therapist.Controllers
{
    [Area("Therapist")]
    [Authorize(Roles = "Therapist")]
    public class ExaminationsController : Controller
    {        
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly ITreatmentsService treatmentsService;
        private readonly ITherapiesService therapiesService;
        private readonly IExaminationsService examinationsService;
        private readonly IAppointmentsService appointmentsService;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public ExaminationsController(
            SignInManager<HCHWebUser> signInManager,
            ITreatmentsService treatmentsService,
            ITherapiesService therapiesService,
            IExaminationsService examinationsService,
            IAppointmentsService appointmentsService,
            IUsersService usersService,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.treatmentsService = treatmentsService;
            this.therapiesService = therapiesService;
            this.examinationsService = examinationsService;
            this.appointmentsService = appointmentsService;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        // GET: Therapist/Examinations
        public async Task<IActionResult> Index(int? page)
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var examinations = await this.examinationsService.AllExaminationsForTherapist(therapistId);

            var examinationsView = examinations
                .OrderByDescending(x => x.ExaminationDate)
                .Select(x => this.mapper.Map<ExaminationViewModel>(x));

            var numberOfItems = 8;

            var pageNumber = page ?? 1;

            var onePageOfExaminations = examinationsView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(onePageOfExaminations);
        }

        // GET: Therapist/Examinations/Details/5
        public async Task<IActionResult> Details(int? id)
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

            var examinationView = this.mapper.Map<ExaminationViewModel>(examination);

            return View(examinationView);
        }

        // GET: Therapist/Examinations/Create/patientId
        public async Task<IActionResult> Create(string id)
        {
            var appointmentId = this.HttpContext.Request.Query["AppointmentId"].ToString();

            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            HCHWebUser patient = this.usersService.GetUserById(id);

            var treatmentsFromProfile = await this.treatmentsService.AllFromProfileAsync(therapist.ProfileId);

            ExaminationInputViewModel examinationViewModel = new ExaminationInputViewModel
            {
                AppointmentId = appointmentId,
                ExaminationDate = DateTime.Today,
                PatientId = id,
                Patient = patient.FirstName + " " + patient.LastName,
                TherapistId = therapistId,
                Therapist = therapist.FirstName + " " + therapist.LastName,
                Anamnesis = "Обективно състояние: ",
                Treatments = treatmentsFromProfile.Select(x => this.mapper.Map<TherapyTreatmentViewModel>(x)).ToList(),
                TherapyStartDate = DateTime.Today
            };

            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(examinationViewModel);
        }

        // POST: Therapist/Examinations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExaminationInputViewModel examinationViewModel)
        {
            if (ModelState.IsValid)
            {
                Therapy therapy = null;
                if (examinationViewModel.Treatments.Any(t => t.Selected == true))
                {
                    if (examinationViewModel.TherapyStartDate < examinationViewModel.ExaminationDate)
                      {
                          this.ModelState.AddModelError("TherapyStartDate", "Невалидна дата за начало на    терапията.");

                          var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList    ();

                          return this.View("Error", errors);
                      }

                    if (examinationViewModel.TherapyDuration == 0)
                    {
                        this.ModelState.AddModelError("TherapyDuration", "Невалиден брой дни.");

                        var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                        return this.View("Error", errors);
                    }

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

                var examination = this.mapper.Map<Examination>(examinationViewModel);

                examination.Therapy = therapy;

                var appointment = await this.appointmentsService.GetAppointmentByIdAsync(examinationViewModel.AppointmentId);

                examination.Price = appointment.Price;

                await this.examinationsService.AddExaminationAsync(examination);                

                await this.appointmentsService.ReleaseAppointmentAsync(examinationViewModel.AppointmentId);

                return RedirectToAction("Index", "Examinations");
            }

            var therapistId = this.signInManager.UserManager.GetUserId(User);
            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(examinationViewModel);
        }

        // GET: Therapist/Examinations/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            var examinationView = this.mapper.Map<ExaminationViewModel>(examination);

            return View(examinationView);
        }

        // POST: Therapist/Examinations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ExaminationViewModel examinationView)
        {            
            if (ModelState.IsValid)
            {
                try
                {
                    var examination = this.mapper.Map<Examination>(examinationView);

                    await this.examinationsService.UpdateExaminationAsync(examination);                   
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.examinationsService.ExaminationExists(examinationView.Id))
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

            return View(examinationView);
        }        

    }
}

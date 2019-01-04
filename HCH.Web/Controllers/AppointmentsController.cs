using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Web.Models;
using AutoMapper;

namespace HCH.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentsService appointmentsService;
        private readonly IUsersService usersService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public AppointmentsController(
            IAppointmentsService appointmentsService,
            IUsersService usersService,
            SignInManager<HCHWebUser> signInManager,
            IMapper mapper)
        {
            this.appointmentsService = appointmentsService;
            this.usersService = usersService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        // GET: Appointments
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Index()
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var appointmentsTherapist = await this.appointmentsService.AppointmentsForTherapistAsync(therapistId);

            var appointmentsView = appointmentsTherapist
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => this.mapper.Map<AppointmentViewModel>(x))            
            .ToList();

            ViewData["TherapistId"] = therapist.Id;
            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(appointmentsView);
        }

        // GET: Appointments/Index_Occupied
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Index_Occupied()
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var appointmentsTherapist = await this.appointmentsService.OccupiedAppointmentsForTherapistAsync(therapistId);

            var appointmentsView = appointmentsTherapist
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => this.mapper.Map<AppointmentViewModel>(x))
                .ToList();

            ViewData["TherapistId"] = therapist.Id;
            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(appointmentsView);
        }

        // GET: Appointments
        [Authorize]
        public async Task<IActionResult> Index_Patient(string id)
        {
            var patientId = this.signInManager.UserManager.GetUserId(User);

            var appointmentsTherapist = await this.appointmentsService.AppointmentsForTherapistAsync(id);

            var appointmentsTherapistView = appointmentsTherapist.Where(x => x.IsFree == true)
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => this.mapper.Map<AppointmentViewModel>(x))
                .ToList();

            HCHWebUser therapist = this.usersService.GetUserById(id);

            ViewData["PatientId"] = patientId;
            ViewData["TherapistId"] = therapist.Id;
            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(appointmentsTherapistView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize()]
        public async Task<IActionResult> Take(string id)
        {
            var patientId = this.signInManager.UserManager.GetUserId(User);

           await this.appointmentsService.TakeAppointmentForPatientAsync(id, patientId);

            return RedirectToAction("Index", "Profiles");
        }

        // GET: Appointments/Details/5
        [Authorize(Roles ="Therapist")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await this.appointmentsService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Therapist")]
        public IActionResult Create(string therapistId)
        {
            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            ViewData["TherapistId"] = therapistId;
            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= "Therapist")]
        public async Task<IActionResult> Create(AppointmentViewModel appointmentModel)
        {
            if (ModelState.IsValid)
            {
                var therapistId = this.signInManager.UserManager.GetUserId(User);

                bool IsThereSuchAppointment = this.appointmentsService.IsThereSuchAppointment(therapistId, appointmentModel.DayOfWeekBg, appointmentModel.VisitingHour);

                if (IsThereSuchAppointment)
                {
                    this.ModelState.AddModelError("Appointment", "Вече има създаден такъв приемен час за този терапевт. Моля изберете друг ден или друг час.");

                    var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                    return this.View("Error", errors);
                }

                Appointment appointmentHCH = this.mapper.Map<Appointment>(appointmentModel);

                await this.appointmentsService.AddAppointmentAsync(appointmentHCH);
                
                return RedirectToAction(nameof(Index));
            }
            ViewData["TherapistId"] = appointmentModel.TherapistId;
            return View(appointmentModel);
        }

        // POST: Appointments/Release/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Release(string id)
        {
            await this.appointmentsService.ReleaseAppointmentAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // POST: Appointments/Release/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> ReleaseAndExaminate(string id)
        {
            await this.appointmentsService.ReleaseAppointmentAsync(id);

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointment appointment = await this.appointmentsService.GetAppointmentByIdAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            AppointmentViewModel appointmentView = this.mapper.Map<AppointmentViewModel>(appointment);

            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);
            
            ViewData["TherapistId"] = therapist.Id;

            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;

            return View(appointmentView);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Edit(string id, AppointmentViewModel appointmentModel)
        {
            if (id != appointmentModel.Id)
            {
                return NotFound();
            }

            var therapistId = this.signInManager.UserManager.GetUserId(User);

            if (ModelState.IsValid)
            {
                bool IsThereSuchAppointment = this.appointmentsService.IsThereSuchAppointment(therapistId, appointmentModel.DayOfWeekBg, appointmentModel.VisitingHour);

                if (IsThereSuchAppointment)
                {
                    this.ModelState.AddModelError("Appointment", "Вече има създаден такъв приемен час за този терапевт. Моля изберете друг ден или друг час.");

                    var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                    return this.View("Error", errors);
                }

                try
                {
                    await this.appointmentsService.UpdateAppointmentAsync(id, appointmentModel.DayOfWeekBg, appointmentModel.VisitingHour);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointmentModel.Id))
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

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            ViewData["TherapistId"] = therapist.Id;

            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;

            return View(appointmentModel);
        }

        // GET: Appointments/Delete/5
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await this.appointmentsService.GetAppointmentByIdAsync(id);
            
            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentView = this.mapper.Map<AppointmentViewModel>(appointment);

            return View(appointmentView);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appointment = await this.appointmentsService.GetAppointmentByIdAsync(id);

            await this.appointmentsService.RemoveAppointmentAsync(appointment);
            
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(string id)
        {
            return this.appointmentsService.ExistsAppointmentById(id);
        }
    }
}

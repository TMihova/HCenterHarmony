using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Web.Models;

namespace HCH.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentsService appointmentsService;
        private readonly IUsersService usersService;
        private readonly SignInManager<HCHWebUser> signInManager;

        public AppointmentsController(
            IAppointmentsService appointmentsService,
            IUsersService usersService,
            SignInManager<HCHWebUser> signInManager)
        {
            this.appointmentsService = appointmentsService;
            this.usersService = usersService;
            this.signInManager = signInManager;
        }

        // GET: Appointments
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> Index(string id)
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser therapist = this.usersService.GetUserById(therapistId);

            var appointmentsTherapist = await this.appointmentsService.AppointmentsForTherapistAsync(therapistId);

            var appointmentsView = appointmentsTherapist
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => new AppointmentViewModel
            {
                Id = x.Id,
                DayOfWeekBg = x.DayOfWeekBg,
                Price = x.Price,
                TherapistId = x.TherapistId,
                VisitingHour = x.VisitingHour,
                TherapistFullName = x.Therapist.FirstName + " " + x.Therapist.LastName,
                PatientId = x.PatientId,
                PatientFullName = x.Patient?.FirstName + " " + x.Patient?.LastName
            }).ToList();

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
                .Select(x => new AppointmentViewModel
                {
                    Id = x.Id,
                    DayOfWeekBg = x.DayOfWeekBg,
                    VisitingHour = x.VisitingHour,
                    TherapistFullName = x.Therapist.FirstName + " " + x.Therapist.LastName,
                    Price = x.Price
                }).ToList();

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

            //HCHWebUser patient = this.usersService.GetUserById(patientId);

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
                //await _context.Appointments
                //.Include(a => a.Patient)
                //.Include(a => a.Therapist)
                //.FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Therapist")]
        public IActionResult Create(string id)
        {
            HCHWebUser therapist = this.usersService.GetUserById(id);

            ViewData["TherapistId"] = id;
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

                Appointment appointmentHCH = new Appointment
                {
                    DayOfWeekBg = appointmentModel.DayOfWeekBg,
                    Price = appointmentModel.Price,
                    TherapistId = appointmentModel.TherapistId,
                    VisitingHour = appointmentModel.VisitingHour
                };

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

            AppointmentViewModel appointmentView = new AppointmentViewModel
            {
                Id = appointment.Id,
                DayOfWeekBg = appointment.DayOfWeekBg,
                VisitingHour = appointment.VisitingHour,
                Price = appointment.Price,
                TherapistId = appointment.TherapistId,
                TherapistFullName = appointment.Therapist.FirstName + " " + appointment.Therapist.LastName,
                PatientId = appointment.PatientId,
                PatientFullName = appointment.Patient?.FirstName + " " + appointment.Patient?.LastName
            };

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

            //var appointment = await _context.Appointments
            //    .Include(a => a.Patient)
            //    .Include(a => a.Therapist)
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentView = new AppointmentViewModel
            {
                Id = appointment.Id,
                DayOfWeekBg = appointment.DayOfWeekBg,
                VisitingHour = appointment.VisitingHour,
                TherapistId = appointment.TherapistId,
                TherapistFullName = appointment.Therapist.FirstName + " " + appointment.Therapist.LastName
            };

            return View(appointmentView);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Therapist")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appointment = await this.appointmentsService.GetAppointmentByIdAsync(id);
            //await _context.Appointments.FindAsync(id);

            await this.appointmentsService.RemoveAppointmentAsync(appointment);
            
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(string id)
        {
            return this.appointmentsService.ExistsAppointmentById(id);
        }
    }
}

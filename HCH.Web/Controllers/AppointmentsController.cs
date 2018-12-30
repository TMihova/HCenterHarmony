using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Data;
using HCH.Services;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Web.Models;

namespace HCH.Web.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly HCHWebContext _context;
        private readonly IAppointmentsService appointmentsService;
        private readonly SignInManager<HCHWebUser> signInManager;

        public AppointmentsController(HCHWebContext context,
            IAppointmentsService appointmentsService,
            SignInManager<HCHWebUser> signInManager)
        {
            _context = context;
            this.appointmentsService = appointmentsService;
            this.signInManager = signInManager;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(string id)
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            var appointmentsTherapist = await this.appointmentsService.AppointmentsForTherapistAsync(id);

            return View(appointmentsTherapist);
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Therapist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        [Authorize(Roles = "Therapist")]
        public IActionResult Create()
        {
            var therapistId = this.signInManager.UserManager.GetUserId(User);

            ViewData["TherapistId"] = therapistId;

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles= "Therapist")]
        public async Task<IActionResult> Create(AppointmentViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", appointment.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", appointment.TherapistId);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", appointment.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", appointment.TherapistId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,DayOfWeekBg,VisitingHour,Price,TherapistId,PatientId")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Users, "Id", "Id", appointment.PatientId);
            ViewData["TherapistId"] = new SelectList(_context.Users, "Id", "Id", appointment.TherapistId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Therapist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(string id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}

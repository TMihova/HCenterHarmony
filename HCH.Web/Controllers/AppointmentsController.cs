using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Models;
using HCH.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Web.Models;
using AutoMapper;
using X.PagedList;

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

        // GET: Appointments/Index_Patient
        [Authorize]
        public async Task<IActionResult> Index_Patient(string id, int? page)
        {
            var patientId = this.signInManager.UserManager.GetUserId(User);

            var appointmentsTherapist = await this.appointmentsService.AppointmentsForTherapistAsync(id);

            var appointmentsTherapistView = appointmentsTherapist.Where(x => x.IsFree == true)
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => this.mapper.Map<AppointmentViewModel>(x))
                .ToList();

            HCHWebUser therapist = this.usersService.GetUserById(id);

            var numberOfItems = 6;

            var pageNumber = page ?? 1;

            var onePageOfAppointments = appointmentsTherapistView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            ViewData["PatientId"] = patientId;
            ViewData["TherapistId"] = therapist.Id;
            ViewData["TherapistFullName"] = therapist.FirstName + " " + therapist.LastName;
            ViewData["ProfileName"] = therapist.Profile.Name;

            return View(onePageOfAppointments);
        }

        // GET: Appointments/Index_Occupied
        [Authorize]
        public async Task<IActionResult> Index_Occupied()
        {
            var userId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser user = this.usersService.GetUserById(userId);

            var appointmentsForPatient = await this.appointmentsService.OccupiedAppointmentsForPatientAsync(userId);

            var appointmentsView = appointmentsForPatient
                .OrderBy(x => x.DayOfWeekBg)
                .ThenBy(x => x.VisitingHour)
                .Select(x => this.mapper.Map<AppointmentViewModel>(x))
                .ToList();

            ViewData["PatientId"] = user.Id;
            ViewData["PatientFullName"] = user.FirstName + " " + user.LastName;

            return View(appointmentsView);
        }

        // POST: Appointments/Take
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Take(string id)
        {
            var patientId = this.signInManager.UserManager.GetUserId(User);

           await this.appointmentsService.TakeAppointmentForPatientAsync(id, patientId);

            return RedirectToAction(nameof(Index_Occupied));
        }

        // POST: Appointments/Release/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Release(string id)
        {
            await this.appointmentsService.ReleaseAppointmentAsync(id);

            return RedirectToAction(nameof(Index_Occupied));
        }
    }
}

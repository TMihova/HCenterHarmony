using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using AutoMapper;
using X.PagedList;

namespace HCH.Web.Controllers
{
    public class ExaminationsController : Controller
    {        
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IExaminationsService examinationsService;
        private readonly IUsersService usersService;
        private readonly IMapper mapper;

        public ExaminationsController(
            SignInManager<HCHWebUser> signInManager,
            IExaminationsService examinationsService,
            IUsersService usersService,
            IMapper mapper)
        {
            this.signInManager = signInManager;
            this.examinationsService = examinationsService;
            this.usersService = usersService;
            this.mapper = mapper;
        }

        // GET: Examinations
        [Authorize]
        public async Task<IActionResult> Index(int? page)
        {
            var userId = this.signInManager.UserManager.GetUserId(User);

            HCHWebUser user = this.usersService.GetUserById(userId);

            var examinations = await this.examinationsService.AllExaminationsForPatientAsync(userId);

            var examinationsView = examinations.Select(x => this.mapper.Map<ExaminationViewModel>(x));

            var numberOfItems = 6;

            var pageNumber = page ?? 1;

            var onePageOfExaminations = examinationsView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            ViewData["PatientFullName"] = user.FirstName + " " + user.LastName;

            return View(onePageOfExaminations);
        }

        // GET: Examinations/Details/5
        [Authorize]
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
    }
}

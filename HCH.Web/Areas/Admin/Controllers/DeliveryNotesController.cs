using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using AutoMapper;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DeliveryNotesController : Controller
    {
        private readonly IOrdersService orderService;
        private readonly IDeliveryNotesService deliveryNotesService;
        private readonly IMapper mapper;

        public DeliveryNotesController(
            IOrdersService orderService,
            IDeliveryNotesService deliveryNotesService,
            IMapper mapper)
        {
            this.orderService = orderService;
            this.deliveryNotesService = deliveryNotesService;
            this.mapper = mapper;
        }

        // GET: Admin/DeliveryNotes
        public async Task<IActionResult> Index()
        {
            var deliveryNotesDb = await this.deliveryNotesService.AllAsync();
            var deliveryNotes = deliveryNotesDb.ToList()
                .Select(x => this.mapper.Map<DeliveryNoteViewModel>(x));

            return View(deliveryNotes);
        }

        // GET: Admin/DeliveryNotes/Create
        public async Task<IActionResult> Create(int id)
        {
            var orderDb = await this.orderService.GetOrderByIdAsync(id);

            var deliveryNoteForOrdeExests = this.deliveryNotesService.IsThereDeliveryNoteForOrder(id);

            if (deliveryNoteForOrdeExests)
            {
                this.ModelState.AddModelError("Exists", "Вече има фактура за тази поръчка.");

                var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                return this.View("Error", errors);
            }

            var cost = orderDb.FoodSupplements.Sum(x => x.ProductCount*x.FoodSupplement.Price);

            ViewData["OrderId"] = id;
            ViewData["OrderDate"] = orderDb.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            ViewData["Cost"] = cost;

            return View();
        }

        // POST: Admin/DeliveryNotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeliveryNoteViewModel deliveryNote)
        {
            if (ModelState.IsValid)
            {
                await this.deliveryNotesService.AddDeliveryNoteForOrder(deliveryNote.OrderId, deliveryNote.Cost, deliveryNote.Discount);

                return RedirectToAction("AllOrders", "Orders");
            }

            return View(deliveryNote.OrderId);
        }
    }
}

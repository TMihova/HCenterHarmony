using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCH.Data;
using HCH.Models;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using System.Globalization;
using System;

namespace HCH.Web.Controllers
{
    public class DeliveryNotesController : Controller
    {
        private readonly IOrdersService orderService;
        private readonly IDeliveryNotesService deliveryNotesService;

        public DeliveryNotesController(
            IOrdersService orderService,
            IDeliveryNotesService deliveryNotesService)
        {
            this.orderService = orderService;
            this.deliveryNotesService = deliveryNotesService;
        }

        // GET: DeliveryNotes/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderId = id.GetValueOrDefault();

            var deliveryNoteViewModel = new DeliveryNoteViewModel();
            deliveryNoteViewModel.OrderId = orderId;

            if (!this.deliveryNotesService.IsThereDeliveryNoteForOrder(orderId))
            {
                deliveryNoteViewModel.Exists = false;

                return this.View(deliveryNoteViewModel);
            }

            var deliveryNoteForOrderDb = await this.deliveryNotesService.GetDeliveryNoteForOrderAsync(orderId);

            ViewData["OrderDate"] = deliveryNoteForOrderDb.Order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            ViewData["ClientName"] = deliveryNoteForOrderDb.Order.Client.FirstName + " " + deliveryNoteForOrderDb.Order.Client.LastName;

            deliveryNoteViewModel.Exists = true;

            deliveryNoteViewModel.Id = deliveryNoteForOrderDb.Id;
            deliveryNoteViewModel.Cost = deliveryNoteForOrderDb.Cost;
            deliveryNoteViewModel.IssueDate = deliveryNoteForOrderDb.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            deliveryNoteViewModel.Discount = deliveryNoteForOrderDb.Discount;

            return View(deliveryNoteViewModel);
        }
        
    }
}

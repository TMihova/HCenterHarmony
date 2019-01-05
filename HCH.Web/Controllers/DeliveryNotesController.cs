using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace HCH.Web.Controllers
{
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

            if (!this.deliveryNotesService.IsThereDeliveryNoteForOrder(orderId))
            {                
                deliveryNoteViewModel.OrderId = orderId;

                deliveryNoteViewModel.Exists = false;

                return this.View(deliveryNoteViewModel);
            }

            var deliveryNoteForOrderDb = await this.deliveryNotesService.GetDeliveryNoteForOrderAsync(orderId);
            
            ViewData["ClientName"] = deliveryNoteForOrderDb.Order.Client.FirstName + " " + deliveryNoteForOrderDb.Order.Client.LastName;

            deliveryNoteViewModel = this.mapper.Map<DeliveryNoteViewModel>(deliveryNoteForOrderDb);

            deliveryNoteViewModel.Exists = true;

            return View(deliveryNoteViewModel);
        }
        
    }
}

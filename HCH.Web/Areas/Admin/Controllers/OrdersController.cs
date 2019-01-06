using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using System.Globalization;
using AutoMapper;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly IDeliveryNotesService deliveryNotesService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public OrdersController(
            IOrdersService ordersService,
            IDeliveryNotesService deliveryNotesService,
            SignInManager<HCHWebUser> signInManager,
            UserManager<HCHWebUser> userManager,
            IMapper mapper)
        {
            this.ordersService = ordersService;
            this.deliveryNotesService = deliveryNotesService;
            this.signInManager = signInManager;
            this.mapper = mapper;
        }

        // GET: Admin/Orders/AllOrders
        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await this.ordersService.AllOrdersAsync();

            var ordersView = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderView = this.mapper.Map<OrderViewModel>(order);

                if (this.deliveryNotesService.IsThereDeliveryNoteForOrder(order.Id))
                {
                    var deliveryNote = await this.deliveryNotesService.GetDeliveryNoteForOrderAsync(order.Id);

                    orderView.DeliveryNoteId = deliveryNote.Id;
                    orderView.DeliveryNoteDate = deliveryNote.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                }

                ordersView.Add(orderView);
            }

            return View(ordersView);
        }

        

        // GET: Admin/Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderId = id.GetValueOrDefault();

            var order = await this.ordersService.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await this.ordersService.GetOrderByIdAsync(id);

            await this.ordersService.RemoveOrderAsync(id);

            return RedirectToAction(nameof(AllOrders));
        }

        private bool OrderExists(int id)
        {
            return this.ordersService.OrderExists(id);
        }
    }
}

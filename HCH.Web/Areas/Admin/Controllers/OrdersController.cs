using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using System.Globalization;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly IDeliveryNotesService deliveryNotesService;
        private readonly SignInManager<HCHWebUser> signInManager;

        public OrdersController(
            IOrdersService ordersService,
            IDeliveryNotesService deliveryNotesService,
            SignInManager<HCHWebUser> signInManager,
            UserManager<HCHWebUser> userManager)
        {
            this.ordersService = ordersService;
            this.deliveryNotesService = deliveryNotesService;
            this.signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> AllOrders()
        {
            var orders = await this.ordersService.AllOrdersAsync();

            var ordersView = new List<OrderViewModel>();

            foreach (var order in orders)
            {

                var orderView = new OrderViewModel
                {
                    Id = order.Id,
                    ClientId = order.ClientId,
                    ClientFullName = order.Client.FirstName + " " + order.Client.LastName,
                    OrderDate = order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    Price = order.FoodSupplements.Sum(x => x.ProductCount * x.FoodSupplement.Price)
                };

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

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderId = id.GetValueOrDefault();

            var order = await this.ordersService.GetOrderByIdAsync(orderId);
                //await _context.Orders.Include(o => o.Client).FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            var orderView = new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                Price = order.FoodSupplements.Sum(x => x.ProductCount * x.FoodSupplement.Price),
                ClientId = order.ClientId,
                ClientFullName = order.Client.FirstName + " " + order.Client.LastName
            };

            return View(orderView);
        }

        // GET: Orders/Delete/5
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

        // POST: Orders/Delete/5
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

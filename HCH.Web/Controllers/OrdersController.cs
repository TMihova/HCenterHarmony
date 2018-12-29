using System;
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

namespace HCH.Web.Controllers
{
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

        

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(int id, OrderViewModel orderViewModel)
        {
            if (ModelState.IsValid)
            {
                var clientId = this.signInManager.UserManager.GetUserId(User);

                var date = DateTime.UtcNow;

                var productCount = int.Parse(this.HttpContext.Request.Form["Count"].ToString());

                if (!this.ordersService.IsThereAnyOrdersForClientOnGivenDate(date, clientId))
                {
                    this.ordersService.AddOrderOnGivenDateToClient(date, clientId, id);
                }

                await this.ordersService.AddFoodSupplementToOrderOnGivenDateAsync(date, clientId, id, productCount);

                return RedirectToAction("Index", "FoodSupplements");
            }
            //ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", orderViewModel.ClientId);
            return RedirectToAction("Index", "FoodSupplements");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderProducts(int id)
        {
            var orderProducts = await this.ordersService.GetOrderProductsByOrderIdAsync(id);
            var order = await this.ordersService.GetOrderByIdAsync(id);

            var orderProductsView = new List<OrderProductViewModel>();

            foreach (var product in orderProducts)
            {
                var productName = product.FoodSupplement.Name;
                

                if (orderProductsView.Any(x => x.Name == productName))
                {
                    orderProductsView.FirstOrDefault(x => x.Name == productName).Count += product.ProductCount;
                }
                else
                {
                    var orderProductView = new OrderProductViewModel
                    {
                        Id = product.FoodSupplement.Id,                        
                        Name = productName,
                        Count = product.ProductCount,
                        Price = product.FoodSupplement.Price
                    };

                    orderProductsView.Add(orderProductView);
                }
            }

            ViewData["OrderId"] = id;
            ViewData["OrderDate"] = order.OrderDate.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            ViewData["TotalPrice"] = order.FoodSupplements.Sum(x => x.FoodSupplement.Price * x.ProductCount);

            return View(orderProductsView);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var clientId = this.signInManager.UserManager.GetUserId(User);

            var orders = await this.ordersService.AllClientOrdersAsync(clientId);

            var ordersView = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                
                var orderView = new OrderViewModel
                {
                    Id = order.Id,
                    ClientId = clientId,
                    OrderDate = order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
                    Price = order.FoodSupplements.Sum(x => x.ProductCount * x.FoodSupplement.Price)
                };

                if (this.deliveryNotesService.IsThereDeliveryNoteForOrder(order.Id))
                {
                    var deliveryNote = await this.deliveryNotesService.GetDeliveryNoteForOrderAsync(order.Id);

                    orderView.DeliveryNoteDate = deliveryNote.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    orderView.DeliveryNoteId = deliveryNote.Id;
                }

                ordersView.Add(orderView);
            }

            return View(ordersView);
        }
        
    }
}

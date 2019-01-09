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
using AutoMapper;
using X.PagedList;

namespace HCH.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrdersService ordersService;
        private readonly IDeliveryNotesService deliveryNotesService;
        private readonly IUsersService usersService;
        private readonly SignInManager<HCHWebUser> signInManager;
        private readonly IMapper mapper;

        public OrdersController(
            IOrdersService ordersService,
            IDeliveryNotesService deliveryNotesService,
            IUsersService usersService,
            SignInManager<HCHWebUser> signInManager,
            IMapper mapper)
        {
            this.ordersService = ordersService;
            this.deliveryNotesService = deliveryNotesService;
            this.usersService = usersService;
            this.signInManager = signInManager;
            this.mapper = mapper;
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

                var successParse = int.TryParse(this.HttpContext.Request.Form["Count"].ToString(), out var productCount);

                if (!successParse || productCount <= 0)
                {
                    this.ModelState.AddModelError("ProductCount", "Невалиден брой продукти.");

                    var errors = ModelState.Values.SelectMany(e => e.Errors).Select(e => e.ErrorMessage).ToList();

                    return this.View("Error", errors);
                }

                if (!this.ordersService.IsThereAnyOrdersForClientOnGivenDate(date, clientId))
                {
                    this.ordersService.AddOrderOnGivenDateToClient(date, clientId);
                }

                await this.ordersService.AddFoodSupplementToOrderOnGivenDateAsync(date, clientId, id, productCount);

                var order = await this.ordersService.GetOrderFromGivenDateAsync(date, clientId);

                var orderView = this.mapper.Map<OrderViewModel>(order);

                return View("Details", orderView);
            }

            return RedirectToAction("Index", "FoodSupplements");
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            var h = this.HttpContext.Request;

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

            var orderView = this.mapper.Map<OrderViewModel>(order);

            return View(orderView);
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
                    var orderProductView = mapper.Map<OrderProductViewModel>(product);

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
        public async Task<IActionResult> MyOrders(int? page)
        {
            var clientId = this.signInManager.UserManager.GetUserId(User);

            var client = this.usersService.GetUserById(clientId);

            var orders = await this.ordersService.AllClientOrdersAsync(clientId);

            var ordersView = new List<OrderViewModel>();

            foreach (var order in orders)
            {
                var orderView = mapper.Map<OrderViewModel>(order);

                if (this.deliveryNotesService.IsThereDeliveryNoteForOrder(order.Id))
                {
                    var deliveryNote = await this.deliveryNotesService.GetDeliveryNoteForOrderAsync(order.Id);

                    orderView.DeliveryNoteDate = deliveryNote.IssueDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
                    orderView.DeliveryNoteId = deliveryNote.Id;
                }

                ordersView.Add(orderView);
            }

            var numberOfItems = 6;

            var pageNumber = page ?? 1;

            var onePageOfOrders = ordersView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            ViewData["ClientFullName"] = client.FirstName + " " + client.LastName;

            return View(onePageOfOrders);
        }
        
    }
}

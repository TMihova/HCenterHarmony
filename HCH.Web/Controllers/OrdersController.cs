using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using HCH.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using HCH.Services;
using System.Globalization;

namespace HCH.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly HCHWebContext _context;
        private readonly IOrdersService ordersService;
        private readonly IDeliveryNotesService deliveryNotesService;
        private readonly SignInManager<HCHWebUser> signInManager;

        public OrdersController(HCHWebContext context,
            IOrdersService ordersService,
            IDeliveryNotesService deliveryNotesService,
            SignInManager<HCHWebUser> signInManager,
            UserManager<HCHWebUser> userManager)
        {
            _context = context;
            this.ordersService = ordersService;
            this.deliveryNotesService = deliveryNotesService;
            this.signInManager = signInManager;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.Orders.Include(o => o.Client);
            return View(await hCHWebContext.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
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

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,ClientId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}

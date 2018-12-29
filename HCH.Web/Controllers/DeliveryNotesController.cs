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
        private readonly HCHWebContext _context;
        private readonly IOrdersService orderService;
        private readonly IDeliveryNotesService deliveryNotesService;

        public DeliveryNotesController(HCHWebContext context,
            IOrdersService orderService,
            IDeliveryNotesService deliveryNotesService)
        {
            _context = context;
            this.orderService = orderService;
            this.deliveryNotesService = deliveryNotesService;
        }

        // GET: DeliveryNotes
        public async Task<IActionResult> Index()
        {
            var hCHWebContext = _context.DeliveryNotes.Include(d => d.Order);
            return View(await hCHWebContext.ToListAsync());
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

        // GET: DeliveryNotes/Create
        [Authorize(Roles = "Admin")]
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

            var date = DateTime.UtcNow;

            ViewData["OrderId"] = id;
            ViewData["OrderDate"] = orderDb.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
            ViewData["Cost"] = cost;

            return View();
        }

        // POST: DeliveryNotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(DeliveryNoteViewModel deliveryNote)
        {
            if (ModelState.IsValid)
            {
                await this.deliveryNotesService.AddDeliveryNoteForOrder(deliveryNote.OrderId, deliveryNote.Cost, deliveryNote.Discount);

                return RedirectToAction("AllOrders", "Orders");
            }

            return View(deliveryNote.OrderId);
        }

        // GET: DeliveryNotes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryNote = await _context.DeliveryNotes.FindAsync(id);
            if (deliveryNote == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", deliveryNote.OrderId);
            return View(deliveryNote);
        }

        // POST: DeliveryNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IssueDate,Cost,Discount,OrderId")] DeliveryNote deliveryNote)
        {
            if (id != deliveryNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(deliveryNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeliveryNoteExists(deliveryNote.Id))
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
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", deliveryNote.OrderId);
            return View(deliveryNote);
        }

        // GET: DeliveryNotes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deliveryNote = await _context.DeliveryNotes
                .Include(d => d.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (deliveryNote == null)
            {
                return NotFound();
            }

            return View(deliveryNote);
        }

        // POST: DeliveryNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deliveryNote = await _context.DeliveryNotes.FindAsync(id);
            _context.DeliveryNotes.Remove(deliveryNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeliveryNoteExists(int id)
        {
            return _context.DeliveryNotes.Any(e => e.Id == id);
        }
    }
}

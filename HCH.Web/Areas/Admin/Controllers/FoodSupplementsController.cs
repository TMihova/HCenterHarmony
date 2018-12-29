using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using HCH.Data;
using Microsoft.AspNetCore.Authorization;
using HCH.Services;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FoodSupplementsController : Controller
    {
        private readonly HCHWebContext _context;
        private readonly IFoodSupplementsService foodSupplementsService;

        public FoodSupplementsController(HCHWebContext context,
            IFoodSupplementsService foodSupplementsService)
        {
            _context = context;
            this.foodSupplementsService = foodSupplementsService;
        }

        // GET: FoodSupplements
        public async Task<IActionResult> Index()
        {
            var products = await this.foodSupplementsService.AllAsync();
                //await _context.FoodSupplements.ToListAsync();

            var productsView = products.Select(x => new FoodSupplementViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price
            })
            .ToList();

            return View(productsView);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index_Admin()
        {
            var products = await this.foodSupplementsService.AllAsync();

            var productsView = products.Select(x => new FoodSupplementViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price
            })
            .ToList();

            return View(productsView);
        }

        // GET: FoodSupplements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }



            var foodSupplement = await _context.FoodSupplements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foodSupplement == null)
            {
                return NotFound();
            }

            return View(foodSupplement);
        }

        // GET: FoodSupplements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodSupplements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price")] FoodSupplement foodSupplement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(foodSupplement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(foodSupplement);
        }

        // GET: FoodSupplements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodSupplement = await _context.FoodSupplements.FindAsync(id);
            if (foodSupplement == null)
            {
                return NotFound();
            }
            return View(foodSupplement);
        }

        // POST: FoodSupplements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price")] FoodSupplement foodSupplement)
        {
            if (id != foodSupplement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(foodSupplement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodSupplementExists(foodSupplement.Id))
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
            return View(foodSupplement);
        }

        // GET: FoodSupplements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var foodSupplement = await _context.FoodSupplements
                .FirstOrDefaultAsync(m => m.Id == id);
            if (foodSupplement == null)
            {
                return NotFound();
            }

            return View(foodSupplement);
        }

        // POST: FoodSupplements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodSupplement = await _context.FoodSupplements.FindAsync(id);
            _context.FoodSupplements.Remove(foodSupplement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodSupplementExists(int id)
        {
            return _context.FoodSupplements.Any(e => e.Id == id);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using HCH.Data;
using Microsoft.AspNetCore.Authorization;
using HCH.Services;

namespace HCH.Web.Controllers
{
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
        
    }
}

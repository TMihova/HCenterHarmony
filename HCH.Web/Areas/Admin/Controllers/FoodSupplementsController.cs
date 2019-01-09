using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HCH.Models;
using HCH.Web.Models;
using Microsoft.AspNetCore.Authorization;
using HCH.Services;
using AutoMapper;
using X.PagedList;

namespace HCH.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class FoodSupplementsController : Controller
    {
        private readonly IFoodSupplementsService foodSupplementsService;
        private readonly IMapper mapper;

        public FoodSupplementsController(
            IFoodSupplementsService foodSupplementsService,
            IMapper mapper)
        {
            this.foodSupplementsService = foodSupplementsService;
            this.mapper = mapper;
        }

        // GET: Admin/FoodSupplements/Index_Admin
        public async Task<IActionResult> Index_Admin(int? page)
        {
            var products = await this.foodSupplementsService.AllAsync();

            var productsView = products.Select(x => mapper.Map<FoodSupplementViewModel>(x)).ToList();

            var numberOfItems = 4;

            var pageNumber = page ?? 1;

            var onePageOfProducts = productsView.ToPagedList(pageNumber, numberOfItems);

            ViewBag.PageNumber = pageNumber;

            ViewBag.NumberOfItems = numberOfItems;

            return View(onePageOfProducts);
        }

        // GET: Admin/FoodSupplements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FoodSupplements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FoodSupplementViewModel foodSupplementModel)
        {
            if (ModelState.IsValid)
            {
                FoodSupplement foodSupplement = mapper.Map<FoodSupplement>(foodSupplementModel);

                await this.foodSupplementsService.AddProductAsync(foodSupplement);
                
                return RedirectToAction(nameof(Index_Admin));
            }
            return View(foodSupplementModel);
        }

        // GET: Admin/FoodSupplements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productId = id.GetValueOrDefault();

            var foodSupplement = await this.foodSupplementsService.GetProductById(productId);

            if (foodSupplement == null)
            {
                return NotFound();
            }

            var foodSupplementView = mapper.Map<FoodSupplementViewModel>(foodSupplement);

            return View(foodSupplementView);
        }

        // POST: Admin/FoodSupplements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FoodSupplementViewModel foodSupplementModel)
        {
            if (id != foodSupplementModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    await this.foodSupplementsService.UpdateProductAsync(id, foodSupplementModel.Name, foodSupplementModel.Price, foodSupplementModel.Description);
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!this.foodSupplementsService.FoodSupplementExists(foodSupplementModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index_Admin));
            }
            return View(foodSupplementModel);
        }

        // GET: Admin/FoodSupplements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productId = id.GetValueOrDefault();

            var foodSupplement = await this.foodSupplementsService.GetProductById(productId);

            if (foodSupplement == null)
            {
                return NotFound();
            }

            var foodSupplementView = mapper.Map<FoodSupplementViewModel>(foodSupplement);

            return View(foodSupplementView);
        }

        // POST: Admin/FoodSupplements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodSupplement = await this.foodSupplementsService.GetProductById(id);

            await this.foodSupplementsService.RemoveProductAsync(foodSupplement);
            
            return RedirectToAction(nameof(Index_Admin));
        }
    }
}

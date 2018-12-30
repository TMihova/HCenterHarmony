﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HCH.Web.Models;
using HCH.Services;
using HCH.Models;
using AutoMapper;

namespace HCH.Web.Controllers
{
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

        // GET: FoodSupplements
        public async Task<IActionResult> Index()
        {
            var products = await this.foodSupplementsService.AllAsync();

            var productsView = products.Select(x => mapper.Map<FoodSupplementViewModel>(x)).ToList();

            return View(productsView);
        }

        // GET: FoodSupplements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productId = id.GetValueOrDefault();

            FoodSupplement foodSupplement = await this.foodSupplementsService.GetProductById(productId);

            if (foodSupplement == null)
            {
                return NotFound();
            }

            var productView = mapper.Map<FoodSupplementViewModel>(foodSupplement);

            return View(foodSupplement);
        }
        
    }
}

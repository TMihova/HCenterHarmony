using AutoMapper;
using HCH.Services;
using HCH.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Web.ViewComponents
{
    public class OrderProductsViewComponent : ViewComponent
    {
        private readonly IOrdersService ordersService;
        private readonly IMapper mapper;

        public OrderProductsViewComponent(
            IOrdersService ordersService,
            IMapper mapper)
        {
            this.ordersService = ordersService;
            this.mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
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
    }
}

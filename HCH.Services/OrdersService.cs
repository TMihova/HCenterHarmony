using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly HCHWebContext context;

        public OrdersService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddFoodSupplementToOrderOnGivenDateAsync(DateTime date, string clientId, int productId, int productCount)
        {            
            var order = await this.context.Orders
                .Where(x => x.ClientId == clientId && x.OrderDate.Date == date.Date)
                .Include(x => x.FoodSupplements)
                .FirstOrDefaultAsync();

            var foodSupplement = await this.context.FoodSupplements.FindAsync(productId);

            var orderFoodSupplement = new OrderFoodSupplement
            {
                OrderId = order.Id,
                FoodSupplementId = productId,
                ProductCount = productCount
            };

            this.context.OrderFoodSupplements.Add(orderFoodSupplement);
            this.context.SaveChanges();
        }
        
        public void AddOrderOnGivenDateToClient(DateTime date, string clientId, int id)
        {
            var order = new Order
            {
                OrderDate = date,
                ClientId = clientId
            };

            this.context.Orders.Add(order);
            this.context.SaveChanges();
        }

        public bool IsThereAnyOrdersForClientOnGivenDate(DateTime date, string clientId)
        {
           return this.context.Orders.Any(x => x.ClientId == clientId && x.OrderDate.Date == date.Date);
        }

        public async Task<IEnumerable<Order>> AllClientOrdersAsync(string clientId)
        {
            return await this.context.Orders
                .Where(x => x.ClientId == clientId).ToListAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await this.context.Orders.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<OrderFoodSupplement>> GetOrderProductsByOrderIdAsync(int id)
        {
            return await this.context.OrderFoodSupplements.Where(x => x.OrderId == id).ToListAsync();
        }

        public async Task<IEnumerable<Order>> AllOrdersAsync()
        {
            return await this.context.Orders.ToListAsync();
        }

        public async Task RemoveOrderAsync(int id)
        {
            var order = await this.context.Orders.FirstOrDefaultAsync(x => x.Id == id);
           this.context.Orders.Remove(order);
            await this.context.SaveChangesAsync();
        }

        public bool OrderExists(int id)
        {
            return this.context.Orders.Any(e => e.Id == id);
        }

        public async Task<int> GetOrderIdFromGivenDateAsync(DateTime date, string clientId)
        {
            var order = await this.context.Orders.FirstOrDefaultAsync(x => x.ClientId == clientId && x.OrderDate.Date == date.Date);

            return order.Id;
        }

        public async Task<Order> GetOrderFromGivenDateAsync(DateTime date, string clientId)
        {
            return await this.context.Orders.FirstOrDefaultAsync(x => x.ClientId == clientId && x.OrderDate.Date == date.Date);
        }
    }
}

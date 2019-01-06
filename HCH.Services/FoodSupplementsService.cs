using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;

namespace HCH.Services
{
    public class FoodSupplementsService : IFoodSupplementsService
    {
        private readonly HCHWebContext context;

        public FoodSupplementsService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddProductAsync(FoodSupplement foodSupplement)
        {
            this.context.FoodSupplements.Add(foodSupplement);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FoodSupplement>> AllAsync()
        {
            return await this.context.FoodSupplements.ToListAsync();
        }

        public Task<FoodSupplement> GetProductById(int productId)
        {
            return this.context.FoodSupplements.FirstOrDefaultAsync(x => x.Id == productId);
        }

        public async Task RemoveProductAsync(FoodSupplement foodSupplement)
        {
            this.context.FoodSupplements.Remove(foodSupplement);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(int id, string name, decimal price, string description)
        {
            var foodSupplement = await this.context.FoodSupplements.FindAsync(id);

            foodSupplement.Name = name;
            foodSupplement.Price = price;
            foodSupplement.Description = description;

            this.context.Update(foodSupplement);
            await this.context.SaveChangesAsync();
        }

        public bool FoodSupplementExists(int id)
        {
            return this.context.FoodSupplements.Any(e => e.Id == id);
        }
    }
}

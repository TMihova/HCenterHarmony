using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<FoodSupplement>> AllAsync()
        {
            return await this.context.FoodSupplements.ToListAsync();
        }
    }
}

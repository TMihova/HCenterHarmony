using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IFoodSupplementsService
    {
        Task<IEnumerable<FoodSupplement>> AllAsync();

        Task<FoodSupplement> GetProductById(int productId);

        Task AddProductAsync(FoodSupplement foodSupplement);

        Task UpdateProductAsync(int id, string name, decimal price, string description);

        Task RemoveProductAsync(FoodSupplement foodSupplement);

        bool FoodSupplementExists(int id);
    }
}
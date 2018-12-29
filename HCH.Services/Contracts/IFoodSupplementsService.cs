using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IFoodSupplementsService
    {
        Task<IEnumerable<FoodSupplement>> AllAsync();
    }
}
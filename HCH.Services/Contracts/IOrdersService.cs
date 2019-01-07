using HCH.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HCH.Services
{
    public interface IOrdersService
    {
        Task AddFoodSupplementToOrderOnGivenDateAsync(DateTime date, string clientId, int productId, int productCount);

        bool IsThereAnyOrdersForClientOnGivenDate(DateTime date, string clientId);

        void AddOrderOnGivenDateToClient(DateTime date, string clientId);

        Task<IEnumerable<Order>> AllClientOrdersAsync(string clientId);

        Task<IEnumerable<Order>> AllOrdersAsync();

        Task<Order> GetOrderByIdAsync(int id);

        Task<IEnumerable<OrderFoodSupplement>> GetOrderProductsByOrderIdAsync(int id);

        Task RemoveOrderAsync(int id);

        bool OrderExists(int id);

        Task<Order> GetOrderFromGivenDateAsync(DateTime date, string clientId);
    }
}

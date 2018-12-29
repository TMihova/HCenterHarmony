using HCH.Models;
using System.Threading.Tasks;

namespace HCH.Services
{
    public interface IDeliveryNotesService
    {
        Task<DeliveryNote> GetDeliveryNoteForOrderAsync(int orderId);
        bool IsThereDeliveryNoteForOrder(int orderId);
        Task AddDeliveryNoteForOrder(int orderId, decimal cost, decimal discount);
    }
}
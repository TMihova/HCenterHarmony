using HCH.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HCH.Services
{
    public interface IDeliveryNotesService
    {
        Task<DeliveryNote> GetDeliveryNoteForOrderAsync(int orderId);

        Task<DeliveryNote> GetDeliveryNoteByIdAsync(int deliveryNoteId);

        bool IsThereDeliveryNoteForOrder(int orderId);

        Task AddDeliveryNoteForOrder(int orderId, decimal cost, decimal discount);

        Task<IEnumerable<DeliveryNote>> AllAsync();

        Task RemoveDeliveryNoteAsync(DeliveryNote deliveryNote);

        bool DeliveryNoteExists(int id);
    }
}
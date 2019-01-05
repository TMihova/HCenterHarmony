using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class DeliveryNotesService : IDeliveryNotesService
    {
        private readonly HCHWebContext context;

        public DeliveryNotesService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddDeliveryNoteForOrder(int orderId, decimal cost, decimal discount)
        {
            var deliveryNote = new DeliveryNote
            {
                IssueDate = DateTime.UtcNow,
                OrderId = orderId,
                Cost = cost,
                Discount = discount
            };

            this.context.DeliveryNotes.Add(deliveryNote);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<DeliveryNote>> AllAsync()
        {
            return await this.context.DeliveryNotes.ToListAsync();
        }

        public async Task<DeliveryNote> GetDeliveryNoteForOrderAsync(int orderId)
        {
            var deliveryNote = await this.context.DeliveryNotes.FirstOrDefaultAsync(x => x.OrderId == orderId);

            return deliveryNote;
        }

        public async Task<DeliveryNote> GetDeliveryNoteByIdAsync(int deliveryNoteId)
        {
            var deliveryNote = await this.context.DeliveryNotes.FirstOrDefaultAsync(x => x.Id == deliveryNoteId);

            return deliveryNote;
        }

        public async Task RemoveDeliveryNoteAsync(DeliveryNote deliveryNote)
        {
            this.context.DeliveryNotes.Remove(deliveryNote);
            await this.context.SaveChangesAsync();
        }

        public bool IsThereDeliveryNoteForOrder(int orderId)
        {
            return this.context.DeliveryNotes.Any(x => x.OrderId == orderId);
        }

        public bool DeliveryNoteExists(int id)
        {
            return this.context.DeliveryNotes.Any(e => e.Id == id);
        }
    }
}

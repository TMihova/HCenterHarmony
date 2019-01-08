using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class TreatmentsService : ITreatmentsService
    {
        private readonly HCHWebContext context;

        public TreatmentsService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddTreatmentAsync(Treatment treatment)
        {           
            this.context.Treatments.Add(treatment);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Treatment>> AllAsync()
        {
            return await this.context.Treatments.ToListAsync();
        }

        public async Task<IEnumerable<Treatment>> AllFromProfileAsync(string profileId)
        {
            return await this.context.Treatments.Where(x => x.ProfileId == profileId).ToListAsync();
        }

        public async Task<Treatment> GetTreatmentById(string id)
        {
            return await this.context.Treatments.FindAsync(id);
        }

        public async Task RemoveTreatmentAsync(Treatment treatment)
        {
            if (this.context.Treatments.Contains(treatment))
            {
                this.context.Treatments.Remove(treatment);
                await this.context.SaveChangesAsync();
            }
        }

        public bool TreatementExists(string id)
        {
            return this.context.Treatments.Any(e => e.Id == id);
        }

        public async Task UpdateTreatmentAsync(Treatment treatment)
        {
            if (this.context.Treatments.Contains(treatment))
            {
                this.context.Update(treatment);
                await this.context.SaveChangesAsync();
            }
        }
    }
}

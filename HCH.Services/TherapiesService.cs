using HCH.Data;
using HCH.Models;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class TherapiesService : ITherapiesService
    {
        private readonly HCHWebContext context;

        public TherapiesService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task<Therapy> GetTherapyByIdAsync(string id)
        {
            return await this.context.Therapies.FindAsync(id);
        }

        public async Task AddTherapyAsync(Therapy therapy)
        {
            this.context.Therapies.Add(therapy);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateTherapy(Therapy therapy)
        {
            var therapyTreatments = this.context.TherapyTreatments.Where(x => x.TherapyId == therapy.Id);

            this.context.TherapyTreatments.RemoveRange(therapyTreatments);
            await this.context.SaveChangesAsync();

            this.context.Update(therapy);
            await this.context.SaveChangesAsync();
        }

        public bool TherapyExists(string id)
        {
            return this.context.Therapies.Any(e => e.Id == id);
        }
    }
}

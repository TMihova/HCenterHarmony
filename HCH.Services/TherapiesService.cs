using HCH.Data;
using HCH.Models;
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

        public async Task AddTherapyAsync(Therapy therapy)
        {
            this.context.Therapies.Add(therapy);
            await this.context.SaveChangesAsync();
        }
    }
}

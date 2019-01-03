using HCH.Data;
using HCH.Models;
using System.Threading.Tasks;

namespace HCH.Services
{
    public class ExaminationsService : IExaminationsService
    {
        private readonly HCHWebContext context;

        public ExaminationsService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddExaminationAsync(Examination examination)
        {
            this.context.Examinations.Add(examination);
            await this.context.SaveChangesAsync();
        }
    }
}

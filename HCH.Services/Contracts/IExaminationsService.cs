using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IExaminationsService
    {
        Task AddExaminationAsync(Examination examination);

        Task<IEnumerable<Examination>> AllExaminationsForTherapist(string therapistId);
    }
}
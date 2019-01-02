using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface ITreatmentsService
    {
        Task AddTreatmentAsync(Treatment treatment);

        Task<IEnumerable<Treatment>> AllAsync();

        Task<IEnumerable<Treatment>> AllFromProfileAsync(string profileId);

        Task<Treatment> GetTreatmentById(string id);

        Task UpdateTreatmentAsync(Treatment treatment);

        Task RemoveTreatmentAsync(Treatment treatment);
        bool TreatementExists(string id);
    }
}
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface ITherapiesService
    {
        Task AddTherapyAsync(Therapy therapy);

        Task<Therapy> GetTherapyByIdAsync(string id);

        Task UpdateTherapy(Therapy therapy);

        bool TherapyExists(string id);
    }
}
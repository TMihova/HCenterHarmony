using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface ITherapiesService
    {
        Task AddTherapyAsync(Therapy therapy);
    }
}
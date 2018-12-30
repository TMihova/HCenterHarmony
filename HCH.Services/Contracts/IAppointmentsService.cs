using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IAppointmentsService
    {
        Task<IEnumerable<Appointment>> AppointmentsForTherapistAsync(string id);
    }
}
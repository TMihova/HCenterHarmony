using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;
using HCH.Models.Enums;

namespace HCH.Services
{
    public interface IAppointmentsService
    {
        Task<IEnumerable<Appointment>> AppointmentsForTherapistAsync(string id);

        Task<Appointment> GetAppointmentByIdAsync(string id);

        Task UpdateAppointmentAsync(string id, DayOfWeekBg dayOfWeekBg, string visitingHour);

        bool IsThereSuchAppointment(string therapistId, DayOfWeekBg dayOfWeekBg, string visitingHour);

        Task AddAppointmentAsync(Appointment appointmentHCH);

        Task RemoveAppointmentAsync(Appointment appointment);

        bool ExistsAppointmentById(string id);

        Task TakeAppointmentForPatientAsync(string id, string patientId);

        Task ReleaseAppointmentAsync(string id);

        Task<IEnumerable<Appointment>> OccupiedAppointmentsForTherapistAsync(string therapistId);

        Task<IEnumerable<Appointment>> OccupiedAppointmentsForPatientAsync(string userId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCH.Data;
using HCH.Models;
using HCH.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace HCH.Services
{
    public class AppointmentsService : IAppointmentsService
    {
        private readonly HCHWebContext context;

        public AppointmentsService(HCHWebContext context)
        {
            this.context = context;
        }

        public async Task AddAppointmentAsync(Appointment appointmentHCH)
        {
            this.context.Appointments.Add(appointmentHCH);
            await this.context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> AppointmentsForTherapistAsync(string id)
        {
            return await this.context.Appointments.Where(x => x.TherapistId == id).ToListAsync();
        }

        public bool ExistsAppointmentById(string id)
        {
            return context.Appointments.Any(e => e.Id == id);
        }

        public async Task<Appointment> GetAppointmentByIdAsync(string id)
        {
            return await this.context.Appointments.FindAsync(id);
        }

        public bool IsThereSuchAppointment(string therapistId, DayOfWeekBg dayOfWeekBg, string visitingHour)
        {
            return this.context.Appointments.Any(x => x.DayOfWeekBg == dayOfWeekBg && x.VisitingHour == visitingHour && x.TherapistId == therapistId);
        }

        public async Task RemoveAppointmentAsync(Appointment appointment)
        {
            this.context.Appointments.Remove(appointment);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(string id, DayOfWeekBg dayOfWeekBg, string visitingHour)
        {
            Appointment appointment = await this.context.Appointments.FindAsync(id);

            appointment.DayOfWeekBg = dayOfWeekBg;
            appointment.VisitingHour = visitingHour;

            this.context.Update(appointment);
            await this.context.SaveChangesAsync();
        }
    }
}

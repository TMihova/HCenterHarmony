using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCH.Data;
using HCH.Models;
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

        public async Task<IEnumerable<Appointment>> AppointmentsForTherapistAsync(string id)
        {
            return await this.context.Appointments.Where(x => x.TherapistId == id).ToListAsync();
        }
    }
}

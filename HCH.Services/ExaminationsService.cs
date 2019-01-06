using HCH.Data;
using HCH.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<Examination>> AllExaminationsForTherapist(string therapistId)
        {
            return await this.context.Examinations.Where(x => x.TherapistId == therapistId).ToListAsync();
        }

        public async Task<Examination> GetExaminationByIdAsync(int examinationId)
        {
            return await this.context.Examinations.FindAsync(examinationId);
        }

        public async Task UpdateExaminationAsync(Examination examination)
        {
            this.context.Update(examination);

            await this.context.SaveChangesAsync();
        }

        public bool ExaminationExists(int id)
        {
            return this.context.Examinations.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<Examination>> AllExaminationsForPatientAsync(string userId)
        {
            return await this.context.Examinations.Where(x => x.PatientId == userId).ToListAsync();
        }

        public async Task<int> GetExaminationIdByTherapyIdAsync(string id)
        {
            var examination = await this.context.Examinations.FirstOrDefaultAsync(x => x.TherapyId == id);

            return examination.Id;
        }

        public async Task<DateTime> GetExaminationDateByTherapyIdAsync(string id)
        {
            var examination = await this.context.Examinations.FirstOrDefaultAsync(x => x.TherapyId == id);

            return examination.ExaminationDate;
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HCH.Models;

namespace HCH.Services
{
    public interface IExaminationsService
    {
        Task AddExaminationAsync(Examination examination);

        Task<IEnumerable<Examination>> AllExaminationsForTherapist(string therapistId);

        Task<Examination> GetExaminationByIdAsync(int examinationId);

        Task UpdateExaminationAsync(Examination examination);

        bool ExaminationExists(int id);

        Task<IEnumerable<Examination>> AllExaminationsForPatientAsync(string userId);

        Task<int> GetExaminationIdByTherapyIdAsync(string id);

        Task<DateTime> GetExaminationDateByTherapyIdAsync(string id);
    }
}
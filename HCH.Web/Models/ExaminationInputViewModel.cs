using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class ExaminationInputViewModel
    {
        public ExaminationInputViewModel()
        {
            this.Treatments = new List<TherapyTreatmentViewModel>();
        }       
        
        [Display(Name = "Дата на прегледа")]
        public DateTime ExaminationDate { get; set; }
       
        public string TherapistId { get; set; }        
        
        public string PatientId { get; set; }

        [Display(Name = "Пациент")]
        public string Patient { get; set; } 
        
        public string TherapyId { get; set; }

        [Display(Name = "Терапевт")]
        public string Therapist { get; set; }

        [Display(Name = "Анамнеза")]
        public string Anamnesis { get; set; }

        [Display(Name = "Начална дата")]
        public DateTime TherapyStartDate { get; set; }

        [Display(Name = "Продължителност в дни")]
        [Range(0, Double.MaxValue, ErrorMessage ="Невалиден брой дни.")]
        public int TherapyDuration { get; set; }

        public List<TherapyTreatmentViewModel> Treatments { get; set; }


    }
}

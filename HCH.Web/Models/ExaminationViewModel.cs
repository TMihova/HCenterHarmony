using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class ExaminationViewModel
    {
        public int Id { get; set; }
       
        [Display(Name ="Дата на прегледа")]
        public DateTime ExaminationDate { get; set; }

        public string TherapistId { get; set; }

        [Display(Name = "Терапевт")]
        public string Therapist { get; set; }

        public string PatientId { get; set; }

        [Display(Name = "Пациент")]
        public string Patient { get; set; }

        [Display(Name = "Анамнеза")]
        public string Anamnesis { get; set; }

        public string TherapyId { get; set; }

        public string Profile { get; set; }

        [Display(Name = "Цена")]
        public decimal Price { get; set; }
    }
}

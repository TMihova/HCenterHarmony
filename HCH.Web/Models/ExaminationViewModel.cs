using HCH.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class ExaminationViewModel
    {
        public int Id { get; set; }
       
        [Required]
        [Display(Name ="Дата на прегледа")]
        public DateTime ExaminationDate { get; set; }

        public string TherapistId { get; set; }

        [Required]
        [Display(Name = "Терапевт")]
        public string Therapist { get; set; }

        public string PatientId { get; set; }

        [Required]
        [Display(Name = "Пациент")]
        public string Patient { get; set; }

        [Required]
        [Display(Name = "Анамнеза")]
        [MinLength(CommonConstants.AnamnesisMinLength, ErrorMessage = "Анамнезата трябва да е повече от {1} символа.")]
        public string Anamnesis { get; set; }

        public string TherapyId { get; set; }

        public string Profile { get; set; }

        [Required]
        [Display(Name = "Цена")]
        [Range(0, Double.MaxValue, ErrorMessage = "Невалидна цена.")]
        public decimal Price { get; set; }
    }
}

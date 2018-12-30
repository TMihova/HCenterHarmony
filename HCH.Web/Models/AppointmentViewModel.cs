using HCH.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class AppointmentViewModel
    {
        [Required]
        [Display(Name = "Ден от седмицата")]
        public DayOfWeekBg DayOfWeekBg { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{2}:[0-9]{2}$", ErrorMessage ="Невалиден формат за приемен час.")]
        [Display(Name ="Приемен час")]
        public string VisitingHour { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Цената трябва да е по-голяма от 0.")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required]
        public string TherapistId { get; set; }

        [Required]
        [Display(Name = "Име и фамилия")]
        public string TherapistFullNme { get; set; }

        public string PatientId { get; set; }

        
        public string PatientFullNme { get; set; }
    }
}

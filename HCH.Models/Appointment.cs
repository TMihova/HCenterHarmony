using HCH.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCH.Models
{
    public class Appointment
    {
        public string Id { get; set; }

        [Required]
        public DayOfWeekBg DayOfWeekBg { get; set; }

        [Required]
        public string VisitingHour { get; set; }

        [Required]
        public decimal Price { get; set; }

        [NotMapped]
        public bool IsFree => this.Patient == null;

        [ForeignKey("Therapist")]
        [Required]
        public string TherapistId { get; set; }

        public virtual HCHWebUser Therapist { get; set; }


        [ForeignKey("Patient")]
        public string PatientId { get; set; }

        public virtual HCHWebUser Patient { get; set; }
    }
}

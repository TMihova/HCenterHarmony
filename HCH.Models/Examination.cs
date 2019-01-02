using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCH.Models
{
    public class Examination
    {
        public int Id { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        [ForeignKey("Therapist")]
        [Required]
        public string TherapistId { get; set; }

        public virtual HCHWebUser Therapist { get; set; }

        [ForeignKey("Patient")]
        [Required]
        public string PatientId { get; set; }

        public virtual HCHWebUser Patient { get; set; }

        [Required]
        public string Anamnesis { get; set; }

        [ForeignKey("Therapy")]
        public string TherapyId { get; set; }

        public virtual Therapy Therapy { get; set; }
    }
}

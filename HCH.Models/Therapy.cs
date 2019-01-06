using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCH.Models
{
    public class Therapy
    {
        public Therapy()
        {
            this.Treatments = new HashSet<TherapyTreatment>();
        }

        public string Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [ForeignKey("Patient")]
        public string PatientId { get; set; }

        public virtual HCHWebUser Patient { get; set; }

        [ForeignKey("Therapist")]
        [Required]
        public string TherapistId { get; set; }

        public virtual HCHWebUser Therapist { get; set; }

        public virtual ICollection<TherapyTreatment> Treatments { get; set; }

    }
}

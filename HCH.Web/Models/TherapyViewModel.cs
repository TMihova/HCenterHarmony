using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class TherapyViewModel
    {
        public TherapyViewModel()
        {
            this.Treatments = new List<TherapyTreatmentViewModel>();
        }

        public string TherapyId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string PatientId { get; set; }

        public string Patient { get; set; }

        [Required]
        public string TherapistId { get; set; }

        public string Therapist { get; set; }

        public List<TherapyTreatmentViewModel> Treatments { get; set; }
    }
}

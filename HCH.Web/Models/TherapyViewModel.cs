using System;
using System.Collections.Generic;

namespace HCH.Web.Models
{
    public class TherapyViewModel
    {
        public TherapyViewModel()
        {
            this.Treatments = new List<TherapyTreatmentViewModel>();
        }

        public string TherapyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string PatientId { get; set; }

        public string Patient { get; set; }

        public string TherapistId { get; set; }

        public string Therapist { get; set; }

        public List<TherapyTreatmentViewModel> Treatments { get; set; }
    }
}

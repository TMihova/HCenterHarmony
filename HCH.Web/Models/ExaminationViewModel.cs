using HCH.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Web.Models
{
    public class ExaminationViewModel
    {
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExaminationDate { get; set; }

        public string TherapistId { get; set; }

        public string Therapist { get; set; }

        public string PatientId { get; set; }

        public string Patient { get; set; }

        public string Anamnesis { get; set; }

        public string TherapyId { get; set; }
    }
}

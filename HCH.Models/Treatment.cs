using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HCH.Models
{
    public class Treatment
    {
        public Treatment()
        {
            this.Therapies = new HashSet<TherapyTreatment>();
        }

        public string Id { get; set; }

        [ForeignKey("Profile")]
        [Required]
        public string ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        [Required]
        public string Name { get; set; }


        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public virtual ICollection<TherapyTreatment> Therapies { get; set; }
    }
}

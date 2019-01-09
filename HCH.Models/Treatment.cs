using HCH.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [MinLength(CommonConstants.NameMinLength)]
        public string Name { get; set; }

        [MinLength(CommonConstants.TreatmentDescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public virtual ICollection<TherapyTreatment> Therapies { get; set; }
    }
}

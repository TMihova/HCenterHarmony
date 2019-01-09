using HCH.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCH.Models
{
    public class FoodSupplement
    {        
        public int Id { get; set; }

        [Required]
        [MinLength(CommonConstants.NameMinLength)]
        public string Name { get; set; }

        [Required]

        [MinLength(CommonConstants.ProductDescriptionMinLength)]
        public string Description { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }
    }
}

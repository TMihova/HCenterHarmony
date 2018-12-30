using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Web.Models
{
    public class FoodSupplementViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Име")]
        [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MinLength(3, ErrorMessage = "The {0} must be at least {1} characters long.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, Double.MaxValue)]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
    }
}

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
        [MinLength(3, ErrorMessage = "Името трябва да е поне 3 символа.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MinLength(3, ErrorMessage = "Описанието трябва да е поне 3 символа.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Цената трябва да е по-голяма от 0.")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
    }
}

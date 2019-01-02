using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class TreatmentViewModel
    {
        public string Id { get; set; }
        
                
        public string Profile { get; set; }

        [Required]
        public string ProfileId { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        [MinLength(3, ErrorMessage = "Наименованието трябва да е поне 3 символа.")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        [MinLength(3, ErrorMessage = "Описанието трябва да е поне 3 символа.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Цената трябва да е по-голяма от 0.")]
        [Display(Name = "Цена в лв.")]        
        public decimal Price { get; set; }
    }
}

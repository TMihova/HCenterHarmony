using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class ProfileViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        [MinLength(3, ErrorMessage = "Наименованието трябва да е поне 3 символа.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MinLength(3, ErrorMessage = "Описанието трябва да е поне 3 символа.")]
        public string Description { get; set; }
    }
}

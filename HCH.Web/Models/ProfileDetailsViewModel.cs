using HCH.Web.Models.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class ProfileDetailsViewModel
    {


        public string Id { get; set; }

        [Required]
        [Display(Name = "Наименование")]
        [MinLength(CommonConstants.NameMinLength, ErrorMessage = "Наименованието трябва да е поне {1} символа.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Описание")]
        [MinLength(CommonConstants.ProfileDescriptionMinLength, ErrorMessage = "Описанието трябва да е поне {1} символа.")]
        public string Description { get; set; }
    }
}

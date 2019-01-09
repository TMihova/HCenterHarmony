using HCH.Models.Common;
using System.ComponentModel.DataAnnotations;

namespace HCH.Models
{
    public class Profile
    {
        public string Id { get; set; }

        [Required]
        [MinLength(CommonConstants.NameMinLength)]
        public string Name { get; set; }

        [Required]
        [MinLength(CommonConstants.ProfileDescriptionMinLength)]
        public string Description { get; set; }
    }
}
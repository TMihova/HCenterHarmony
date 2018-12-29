using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCH.Models
{
    public class Profile
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
    }
}
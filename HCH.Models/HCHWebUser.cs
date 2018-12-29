using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HCH.Models
{
    // Add profile data for application users by adding properties to the HCHWebUser class
    public class HCHWebUser : IdentityUser
    {
        public HCHWebUser()
        {
            this.Appointments = new HashSet<Appointment>();
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [ForeignKey("Profile")]
        public string ProfileId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}

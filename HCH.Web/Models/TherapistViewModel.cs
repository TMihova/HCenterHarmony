using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class TherapistViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Display(Name = "Име и фамилия")]
        public string FullName { get; set; }

        [Display(Name = "Специалност")]
        public string Profile { get; set; }
    }
}

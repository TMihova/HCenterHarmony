using System;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class DeliveryNoteViewModel
    {
        public bool Exists { get; set; }

        public int Id { get; set; }
        
        public string IssueDate { get; set; }

        public decimal Cost { get; set; }

        [Required]
        [Range(0.00, 1.0, ErrorMessage = "Отстъпката трябва да е между 0.00 и 1.00.")]
        [Display(Name = "Отстъпка")]
        public decimal Discount { get; set; }

        public int OrderId { get; set; }

        public string OrderDate { get; set; }
    }
}

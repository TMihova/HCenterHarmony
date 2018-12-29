using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HCH.Models
{
    public class DeliveryNote
    {
        public int Id { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public decimal Cost { get; set; }

        public decimal Discount { get; set; }

        [Required]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HCH.Models
{
    public class DeliveryNote
    {
        public int Id { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        [Range(0, Double.MaxValue)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Cost { get; set; }

        public decimal Discount { get; set; }

        [Required]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }
    }
}

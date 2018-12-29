using System;

namespace HCH.Web.Models
{
    public class DeliveryNoteViewModel
    {
        public bool Exists { get; set; }

        public int Id { get; set; }
        
        public string IssueDate { get; set; }

        public decimal Cost { get; set; }

        public decimal Discount { get; set; }

        public int OrderId { get; set; }

        public string OrderDate { get; set; }
    }
}

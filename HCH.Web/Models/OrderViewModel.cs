using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HCH.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        
        public string OrderDate { get; set; }
        
        public string ClientId { get; set; }

        public string ClientFullName { get; set; }

        public decimal Price { get; set; }

        public int DeliveryNoteId { get; set; }

        public string DeliveryNoteDate { get; set; }
    }
}

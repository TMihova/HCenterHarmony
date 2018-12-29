using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace HCH.Models
{
    public class Order
    {
        public Order()
        {
            this.FoodSupplements = new List<OrderFoodSupplement>();
        }

        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [ForeignKey("Client")]
        [Required]
        public string ClientId { get; set; }

        public virtual HCHWebUser Client { get; set; }

        public virtual ICollection<OrderFoodSupplement> FoodSupplements { get; set; }

        [NotMapped]
        public decimal Price => this.FoodSupplements.Sum(x => x.FoodSupplement.Price);
    }
}

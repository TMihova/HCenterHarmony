using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HCH.Models
{
    public class OrderFoodSupplement
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Order")]
        [Required]
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("FoodSupplement")]
        [Required]
        public int FoodSupplementId { get; set; }

        public virtual FoodSupplement FoodSupplement { get; set; }

        public int ProductCount { get; set; }
    }
}

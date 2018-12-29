
using System.ComponentModel.DataAnnotations;

namespace HCH.Models
{
    public class FoodSupplement
    {        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}

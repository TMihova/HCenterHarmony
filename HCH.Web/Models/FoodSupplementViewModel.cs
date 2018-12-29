using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HCH.Web.Models
{
    public class FoodSupplementViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}

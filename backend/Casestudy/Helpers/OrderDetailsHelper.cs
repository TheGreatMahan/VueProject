using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Casestudy.Helpers
{
    public class OrderDetailsHelper
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public decimal SellingPrice { get; set; }
        public int QtyS { get; set; }
        public int QtyO { get; set; }
        public int QtyB { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public string DateCreated { get; set; }

    }
}

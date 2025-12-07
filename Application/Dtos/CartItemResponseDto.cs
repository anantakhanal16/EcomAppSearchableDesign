using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class CartItemResponseDto
    {
    
        public int CartItemID { get; set; }
        public int ProductID { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } 

    }
}

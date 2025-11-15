using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class OrderDetailResponseDto
    {
        public int OrderDetailID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal SubTotal { get; set; }
        public string ProductName { get; set; }
        
    }
}

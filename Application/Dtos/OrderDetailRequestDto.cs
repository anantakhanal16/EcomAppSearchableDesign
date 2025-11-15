using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OrderDetailRequestDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }
    }
}

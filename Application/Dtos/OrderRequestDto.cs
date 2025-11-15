using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class OrderRequestDto
    {
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderDetailRequestDto> OrderDetails { get; set; }
    }
}

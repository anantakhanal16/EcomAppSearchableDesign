using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class OrderResponseDto
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public List<OrderDetailResponseDto> OrderDetails { get; set; }
    }
}

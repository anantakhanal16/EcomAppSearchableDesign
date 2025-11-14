using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class ProductResponseDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int SupplierID { get; set; }
        public bool IsActive { get; set; }
    }
}

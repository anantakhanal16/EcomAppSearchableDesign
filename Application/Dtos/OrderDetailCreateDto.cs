using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dtos
{
    public class OrderDetailCreateDto
    {
        [Required(ErrorMessage = "ProductID is required.")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "ProductPrice is required.")]
        public int ProductPrice { get; set; }
        
        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        
        public int Quantity { get; set; }
        [Required(ErrorMessage = "SubTotal is required.")]
        public decimal SubTotal { get; set; }
    }
}

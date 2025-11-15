using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.Dtos
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "OrderDate is required.")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "CustomerName is required.")]
        [StringLength(100, ErrorMessage = "CustomerName cannot exceed 100 characters.")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "CustomerEmail is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string CustomerEmail { get; set; }

        [Required(ErrorMessage = "TotalAmount is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "TotalAmount must be a positive number.")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "OrderDetails are required.")]
        [MinLength(1, ErrorMessage = "At least one order detail is required.")]
        public List<OrderDetailCreateDto> OrderDetails { get; set; }
    }
}


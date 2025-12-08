using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class CreateProductReviewDto
    {
        public int ProductId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}

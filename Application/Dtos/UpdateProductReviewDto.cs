using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class UpdateProductReviewDto
    {
        public int ReviewId { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
    }
}

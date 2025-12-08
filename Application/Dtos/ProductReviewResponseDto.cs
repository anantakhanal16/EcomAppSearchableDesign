using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class ProductReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string ReviewerName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

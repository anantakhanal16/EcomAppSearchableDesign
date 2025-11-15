using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class GetOrdersDto
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

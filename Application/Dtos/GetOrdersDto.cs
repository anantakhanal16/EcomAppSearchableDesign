using System;
using System.Collections.Generic;
using System.Text;
using Application.Helpers;

namespace Application.Dtos
{
    public class GetOrdersDto:PagedRequest
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}

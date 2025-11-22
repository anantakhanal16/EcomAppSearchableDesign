using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Helpers
{
    public class PagedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

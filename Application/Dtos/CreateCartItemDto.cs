using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class CreateCartItemDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }
}

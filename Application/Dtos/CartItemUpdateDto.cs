using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class CartItemUpdateDto
    {
        public int Quantity { get; set; }
        public int CartId { get; set; }

    }
}

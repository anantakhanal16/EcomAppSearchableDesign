using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Dtos
{
    public class CartResponseDto
    {
        public int CartID { get; set; }
        public string UserID { get; set; }
        public List<CartItemResponseDto> Items { get; set; }
    }
}

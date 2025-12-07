using System.Security.Claims;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAppSearchableDesign.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("get-cart")]
        public async Task<HttpResponses<CartResponseDto>> GetCart(CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToErrorResponse<CartResponseDto>();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            return await _cartService.GetCartAsync(userId, cancellationToken);
        }

        [HttpPost("add-item")]
        public async Task<HttpResponses<CartResponseDto>> AddItem([FromBody] CartItemCreateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToErrorResponse<CartResponseDto>();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            return await _cartService.AddItemAsync(userId, dto, cancellationToken);
        }

     
    [HttpPut("update-item")]
    public async Task<HttpResponses<CartResponseDto>> UpdateItem([FromQuery]int id,[FromBody]CartItemUpdateDto dto, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ModelState.ToErrorResponse<CartResponseDto>();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            return await _cartService.UpdateItemAsync(userId, dto.CartId, dto, cancellationToken);
        }

        [HttpDelete("remove-item")]
        public async Task<HttpResponses<string>> RemoveItem([FromQuery] int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return ModelState.ToErrorResponse<string>();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            return await _cartService.RemoveItemAsync(userId, id, cancellationToken);
        }

        [HttpDelete("clear-cart")]
        public async Task<HttpResponses<string>> ClearCart(CancellationToken cancellationToken)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            return await _cartService.ClearCartAsync(userId, cancellationToken);
        }
    }
}

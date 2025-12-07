using System.Data;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HttpResponses<CartResponseDto>> GetCartAsync(string userId, CancellationToken cancellationToken)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId, cancellationToken);

            if (cart == null)
                return HttpResponses<CartResponseDto>.FailResponse("Cart is empty.");

            return HttpResponses<CartResponseDto>.SuccessResponse(MapToDto(cart), "Cart retrieved successfully.");
        }

        public async Task<HttpResponses<CartResponseDto>> AddItemAsync(string userId, CartItemCreateDto dto, CancellationToken cancellationToken)
        {

            try
            {

                var cart = await _context.Cart
                    .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                    .FirstOrDefaultAsync(c => c.UserID == userId, cancellationToken);

                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserID = userId,
                        CreatedAt = DateTime.UtcNow,
                        CartItems = new List<CartItem>()
                    };
                    _context.Cart.Add(cart);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductID == dto.ProductID);
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == dto.ProductID, cancellationToken);

                if (product == null)
                    return HttpResponses<CartResponseDto>.FailResponse("Product not found.");

                if (existingItem != null)
                    existingItem.Quantity += dto.Quantity;
                else
                    cart.CartItems.Add(new CartItem
                    {
                        ProductID = dto.ProductID,
                        Quantity = dto.Quantity,
                        UnitPrice = product.Price,
                        AddedAt = DateTime.UtcNow
                    });

                await _context.SaveChangesAsync(cancellationToken);

                var cartItemDetail = MapToDto(cart);
                return HttpResponses<CartResponseDto>.SuccessResponse(cartItemDetail, "Item added to cart.");
            }
            catch
            {
                return HttpResponses<CartResponseDto>.FailResponse("Failed to add item to cart.");
            }
        }

        public async Task<HttpResponses<CartResponseDto>> UpdateItemAsync(string userId, int cartItemId, CartItemUpdateDto dto, CancellationToken cancellationToken)
        {
            var item = await _context.CartItem
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId && ci.Cart.UserID == userId, cancellationToken);

            if (item == null)
                return HttpResponses<CartResponseDto>.FailResponse("Cart item not found.");

            item.Quantity = dto.Quantity;
            await _context.SaveChangesAsync(cancellationToken);

            return await GetCartAsync(userId, cancellationToken);
        }

        public async Task<HttpResponses<string>> RemoveItemAsync(string userId, int cartItemId, CancellationToken cancellationToken)
        {
            var item = await _context.CartItem
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId && ci.Cart.UserID == userId, cancellationToken);

            if (item == null)
                return HttpResponses<string>.FailResponse("Cart item not found.");

            _context.CartItem.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);

            return HttpResponses<string>.SuccessResponse(null, "Item removed from cart.");
        }

        public async Task<HttpResponses<string>> ClearCartAsync(string userId, CancellationToken cancellationToken)
        {
            var cart = await _context.Cart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserID == userId, cancellationToken);

            if (cart == null || !cart.CartItems.Any())
                return HttpResponses<string>.FailResponse("Cart is already empty.");

            _context.CartItem.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync(cancellationToken);

            return HttpResponses<string>.SuccessResponse(null, "Cart cleared successfully.");
        }

        private static CartResponseDto MapToDto(Cart cart)
        {
            return new CartResponseDto
            {
                CartID = cart.CartID,
                UserID = cart.UserID.ToString(),
                Items = cart.CartItems.Select(ci => new CartItemResponseDto
                {
                    CartItemID = ci.CartItemID,
                    ProductID = ci.ProductID,
                    ProductName = ci.Product.ProductName,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };
        }
    }
}

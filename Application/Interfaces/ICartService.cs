using System;
using System.Collections.Generic;
using System.Text;
using Application.Dtos;
using Application.Helpers;

namespace Application.Interfaces
{
    public interface ICartService
    {
        Task<HttpResponses<CartResponseDto>> GetCartAsync(string userId, CancellationToken cancellationToken);
        Task<HttpResponses<CartResponseDto>> AddItemAsync(string userId, CartItemCreateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<CartResponseDto>> UpdateItemAsync(string userId, int cartItemId, CartItemUpdateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<string>> RemoveItemAsync(string userId, int cartItemId, CancellationToken cancellationToken);
        Task<HttpResponses<string>> ClearCartAsync(string userId, CancellationToken cancellationToken);
    }
}

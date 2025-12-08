using System;
using System.Collections.Generic;
using System.Text;
using Application.Dtos;
using Application.Helpers;

namespace Application.Interfaces
{
    public interface IProductService
    {
        Task<HttpResponses<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<ProductResponseDto>> UpdateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<string>> DeleteProductAsync(int id, CancellationToken cancellationToken);
        Task<HttpResponses<ProductResponseDto>> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<HttpResponses<string>> ImportProductData(Stream fileStream, CancellationToken cancellationToken);
        Task<HttpResponses<PagedResult<ProductResponseDto>>> GetAllProductsAsync(GetAllProductDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<ProductReviewResponseDto>> CreateAsync(CreateProductReviewDto dto, string userId, CancellationToken cancellationToken);
        Task<HttpResponses<ProductReviewResponseDto>> UpdateAsync(UpdateProductReviewDto dto, string userId, CancellationToken cancellationToken);
        Task<HttpResponses<string>> DeleteAsync(int reviewId, CancellationToken cancellationToken);
        Task<HttpResponses<ProductReviewResponseDto>> GetByIdAsync(int reviewId, CancellationToken cancellationToken);
        Task<HttpResponses<List<ProductReviewResponseDto>>> GetByProductIdAsync(int productId, CancellationToken cancellationToken);
    }
}
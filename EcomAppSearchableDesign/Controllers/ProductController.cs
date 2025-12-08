using System.Security.Claims;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAppSearchableDesign.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpPost("create-product")]
    public async Task<HttpResponses<ProductResponseDto>> CreateProduct([FromForm] ProductCreateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<ProductResponseDto>();
        }

        return await productService.CreateProductAsync(dto, cancellationToken);
    }

    [HttpGet("get-all-products")]
    public async Task<HttpResponses<PagedResult<ProductResponseDto>>> GetAllProducts([FromQuery] GetAllProductDto getAllProductDto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<PagedResult<ProductResponseDto>>();
        }

        return await productService.GetAllProductsAsync(getAllProductDto, cancellationToken);
    }

    [HttpGet("get-product/{id:int}")]
    public async Task<HttpResponses<ProductResponseDto>> GetProductById(int id, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            return ModelState.ToErrorResponse<ProductResponseDto>();
        }

        return await productService.GetProductByIdAsync(id, cancellationToken);
    }

    [HttpPut("update-product")]
    public async Task<HttpResponses<ProductResponseDto>> UpdateProduct([FromForm] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<ProductResponseDto>();
        }

        return await productService.UpdateProductAsync(dto, cancellationToken);
    }

    [HttpDelete("delete-product/{id:int}")]
    public async Task<HttpResponses<string>> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            return ModelState.ToErrorResponse<string>();
        }

        return await productService.DeleteProductAsync(id, cancellationToken);
    }

    [HttpPost("ImportProductData")]
    public async Task<HttpResponses<string>> ImportProductData(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return HttpResponses<string>.FailResponse("No file uploaded");

        using var stream = file.OpenReadStream();
        return await productService.ImportProductData(stream, cancellationToken);
    }

    [HttpPost("create-review")]
    public async Task<HttpResponses<ProductReviewResponseDto>> CreateReview([FromBody] CreateProductReviewDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<ProductReviewResponseDto>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return await productService.CreateAsync(dto, userId, cancellationToken);
    }

    [HttpPut("update-review")]
    public async Task<HttpResponses<ProductReviewResponseDto>> UpdateReview([FromBody] UpdateProductReviewDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<ProductReviewResponseDto>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return await productService.UpdateAsync(dto,userId, cancellationToken);
    }

    [HttpDelete("delete-review/{reviewId:int}")]
    public async Task<HttpResponses<string>> DeleteReview(int reviewId, CancellationToken cancellationToken)
    {
        if (reviewId == 0)
        {
            return ModelState.ToErrorResponse<string>();
        }

        return await productService.DeleteAsync(reviewId, cancellationToken);
    }

    [HttpGet("get-review/{reviewId:int}")]
    public async Task<HttpResponses<ProductReviewResponseDto>> GetReviewById(int reviewId, CancellationToken cancellationToken)
    {
        if (reviewId == 0)
        {
            return ModelState.ToErrorResponse<ProductReviewResponseDto>();
        }

        return await productService.GetByIdAsync(reviewId, cancellationToken);
    }

    [HttpGet("get-product-reviews/{productId:int}")]
    public async Task<HttpResponses<List<ProductReviewResponseDto>>> GetReviewsByProductId(int productId, CancellationToken cancellationToken)
    {
        if (productId == 0)
        {
            return ModelState.ToErrorResponse<List<ProductReviewResponseDto>>();
        }

        return await productService.GetByProductIdAsync(productId, cancellationToken);
    }
}
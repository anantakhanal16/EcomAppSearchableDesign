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
    public async Task<HttpResponses<List<ProductResponseDto>>> GetAllProducts(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<List<ProductResponseDto>>();
        }
        return await productService.GetAllProductsAsync(cancellationToken);
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

    [HttpPut("update-product/{id:int}")]
    public async Task<HttpResponses<ProductResponseDto>> UpdateProduct(int id, [FromBody] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            return ModelState.ToErrorResponse<ProductResponseDto>();
        }
        return await productService.UpdateProductAsync(id, dto, cancellationToken);
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
}
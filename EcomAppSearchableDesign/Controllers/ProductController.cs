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
    public async Task<HttpResponses<ProductResponseDto>> CreateProduct([FromBody] ProductCreateDto dto, CancellationToken cancellationToken)
    {
        return await productService.CreateProductAsync(dto, cancellationToken);
    }

    [HttpGet("get-all-products")]
    public async Task<HttpResponses<List<ProductResponseDto>>> GetAllProducts(CancellationToken cancellationToken)
    {
        return await productService.GetAllProductsAsync(cancellationToken);
    }

    [HttpGet("get-product/{id:int}")]
    public async Task<HttpResponses<ProductResponseDto>> GetProductById(int id, CancellationToken cancellationToken)
    {
        return await productService.GetProductByIdAsync(id, cancellationToken);
    }

    [HttpPut("update-product/{id:int}")]
    public async Task<HttpResponses<ProductResponseDto>> UpdateProduct(int id, [FromBody] ProductUpdateDto dto, CancellationToken cancellationToken)
    {
        return await productService.UpdateProductAsync(id, dto, cancellationToken);
    }

    [HttpDelete("delete-product/{id:int}")]
    public async Task<HttpResponses<string>> DeleteProduct(int id, CancellationToken cancellationToken)
    {
        return await productService.DeleteProductAsync(id, cancellationToken);
    }
}
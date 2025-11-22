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
    public async Task<HttpResponses<PagedResult<ProductResponseDto>>> GetAllProducts([FromQuery]GetAllProductDto getAllProductDto ,CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<PagedResult<ProductResponseDto>>();
        }
        return await productService.GetAllProductsAsync(getAllProductDto,cancellationToken);
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
        return await productService.UpdateProductAsync( dto, cancellationToken);
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
        if (file == null || file.Length == 0)
            return HttpResponses<string>.FailResponse("No file uploaded");

        using var stream = file.OpenReadStream();
        return await productService.ImportProductData(stream, cancellationToken);
    }
}
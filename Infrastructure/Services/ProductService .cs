using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpResponses<ProductResponseDto>> CreateProductAsync(
            ProductCreateDto dto,
            CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.SupplierID == dto.SupplierID, cancellationToken);

            if (supplier == null)
            {
                return HttpResponses<ProductResponseDto>.FailResponse("Supplier does not exist.");
            }

            string imagePath = null;
            if (dto.ProductImage != null)
            {
                var imageSavedResult = await FileHelper.SaveProductImageAsync(dto.ProductImage, cancellationToken);
                if (!imageSavedResult.Success)
                {
                    return HttpResponses<ProductResponseDto>.FailResponse(imageSavedResult.Message);
                }
                else
                {
                    imagePath = imageSavedResult.Data;
                }
            }
            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var fullImagePath = imagePath != null ? $"{baseUrl}{imagePath}" : null;

            var product = new Product
            {
                ProductName = dto.ProductName,
                Category = dto.Category,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SupplierID = dto.SupplierID,
                IsActive = dto.IsActive,
                ProductImage = fullImagePath
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new ProductResponseDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SupplierID = product.SupplierID,
                IsActive = product.IsActive,
                ProductImage = product.ProductImage
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product created successfully.");
        }

        public async Task<HttpResponses<string>> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null) return HttpResponses<string>.FailResponse("Product not found.");
            
            var productExistsinOrder = await _context.OrderDetails.AnyAsync(o => o.ProductID == id);
            if (productExistsinOrder)
            {
                return HttpResponses<string>.FailResponse("Product exists in order . so deletion is prohibited. ");
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);
            return HttpResponses<string>.SuccessResponse(null, "Product deleted successfully.");
        }

        public async Task<HttpResponses<List<ProductResponseDto>>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            var products = await _context.Products.Select(p => new ProductResponseDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Category = p.Category,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                SupplierID = p.SupplierID,
                IsActive = p.IsActive
            }).ToListAsync(cancellationToken);

            return HttpResponses<List<ProductResponseDto>>.SuccessResponse(products, "Products retrieved successfully.");
        }

        public async Task<HttpResponses<ProductResponseDto>> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null) return HttpResponses<ProductResponseDto>.FailResponse("Product not found.");

            var response = new ProductResponseDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SupplierID = product.SupplierID,
                IsActive = product.IsActive
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product retrieved successfully.");
        }

        public async Task<HttpResponses<ProductResponseDto>> UpdateProductAsync(int id, ProductUpdateDto dto, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null) return HttpResponses<ProductResponseDto>.FailResponse("Product not found.");
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == dto.SupplierID);
            if (supplier == null)
            {
                return HttpResponses<ProductResponseDto>.FailResponse("Supplier doesnot exists.");
            }

            product.ProductName = dto.ProductName;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.SupplierID = dto.SupplierID;
            product.IsActive = dto.IsActive;

            _context.Products.Update(product);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new ProductResponseDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                Category = product.Category,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                SupplierID = product.SupplierID,
                IsActive = product.IsActive
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product updated successfully.");
        }
    }
}
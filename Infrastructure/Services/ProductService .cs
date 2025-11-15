using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HttpResponses<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == dto.SupplierID);
            if (supplier == null)
            {
                return HttpResponses<ProductResponseDto>.FailResponse("Supplier doesnot exists.");
            }

            var product = new Product
            {
                ProductName = dto.ProductName,
                Category = dto.Category,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SupplierID = dto.SupplierID,
                IsActive = dto.IsActive
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
                IsActive = product.IsActive
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product created successfully.");
        }

        public async Task<HttpResponses<string>> DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FindAsync(new object[] { id }, cancellationToken);
            if (product == null) return HttpResponses<string>.FailResponse("Product not found.");

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
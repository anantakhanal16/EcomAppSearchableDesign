using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using ClosedXML.Excel;
using Core.Entities;
using Domain.Entities;
using Humanizer;
using iText.Commons.Actions.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(AppDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<HttpResponses<ProductResponseDto>> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken)
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == dto.SupplierID, cancellationToken);

            if (supplier == null)
            {
                return HttpResponses<ProductResponseDto>.FailResponse("Supplier does not exist.");
            }

            string imagePath = "";
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
            var imageUrl = imagePath != null ? $"{baseUrl}{imagePath}" : "";

            var product = new Product
            {
                ProductName = dto.ProductName,
                Category = dto.Category,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SupplierID = dto.SupplierID,
                IsActive = dto.IsActive,
                ProductImage = imageUrl
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new ProductResponseDto
            {
                ProductID = product.ProductID,
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

        public async Task<HttpResponses<PagedResult<ProductResponseDto>>> GetAllProductsAsync(GetAllProductDto dto, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();

            var totalCount = await query.CountAsync(cancellationToken);

            var products = await query.Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize).Select(p => new ProductResponseDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Category = p.Category,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                SupplierID = p.SupplierID,
                IsActive = p.IsActive,
                ProductImage = p.ProductImage
            }).ToListAsync(cancellationToken);

            var result = new PagedResult<ProductResponseDto>
            {
                Items = products,
                TotalCount = totalCount,
                PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };

            return HttpResponses<PagedResult<ProductResponseDto>>.SuccessResponse(result, "Products retrieved successfully.");
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
                IsActive = product.IsActive,
                ProductImage = product.ProductImage
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product retrieved successfully.");
        }

        public async Task<HttpResponses<string>> ImportProductData(Stream fileStream, CancellationToken cancellationToken)
        {
            var productsToInsert = new List<Product>();
            var validationErrors = new List<string>();

            using (var workbook = new XLWorkbook(fileStream))
            {
                var worksheet = workbook.Worksheet(1);
                var rows = worksheet.RowsUsed();

                foreach (var row in rows.Skip(1))
                {
                    var isValid = TryValidateRow(row);
                    if (!isValid.Success)
                    {
                        validationErrors.Add($"Row {row.RowNumber()}:" + isValid.Message);
                        continue;
                    }

                    var productDto = new ImportExcelProductDto
                    {
                        ProductName = row.Cell(1).GetValue<string>(),
                        Category = row.Cell(2).GetValue<string>(),
                        Price = row.Cell(3).GetValue<decimal>(),
                        StockQuantity = row.Cell(4).GetValue<int>(),
                        SupplierID = row.Cell(5).GetValue<int>(),
                        IsActive = row.Cell(6).GetValue<bool>(),
                        ProductUrl = row.Cell(7).GetValue<string>(),
                    };
                    var supplierExists = await _context.Suppliers.AnyAsync(s => s.SupplierID == productDto.SupplierID);
                    if (!supplierExists)
                    {
                        validationErrors.Add($"Row {row.RowNumber()}: " + $"Failed to insert {productDto.ProductName} as supplier doesnot exists");
                        continue;
                    }

                    bool checkProductAlreadyexists = await _context.Products.AnyAsync(p => p.ProductName == productDto.ProductName && p.Category == productDto.Category && p.SupplierID == productDto.SupplierID, cancellationToken);

                    if (checkProductAlreadyexists)
                    {
                        validationErrors.Add($"Row {row.RowNumber()}: Product '{productDto.ProductName}' already exists for same category & supplier.");
                        continue;
                    }

                    var product = new Product
                    {
                        ProductName = productDto.ProductName,
                        Category = productDto.Category,
                        Price = productDto.Price,
                        StockQuantity = productDto.StockQuantity,
                        SupplierID = productDto.SupplierID,
                        IsActive = productDto.IsActive,
                        ProductImage = productDto.ProductUrl
                    };

                    productsToInsert.Add(product);
                }
            }

            if (productsToInsert.Any())
            {
                await _context.Products.AddRangeAsync(productsToInsert, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var errors = "";
            if (validationErrors.Any())
            {
                errors = $" some rows failed to insert :\n{string.Join("\n", validationErrors)}";
            }

            return HttpResponses<string>.SuccessResponse($"Successfully imported {productsToInsert.Count} products ," + errors);
        }

        public async Task<HttpResponses<ProductResponseDto>> UpdateProductAsync(ProductUpdateDto dto, CancellationToken cancellationToken)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == dto.Id, cancellationToken);
            if (product == null) return HttpResponses<ProductResponseDto>.FailResponse("Product not found.");

            var supplier = await _context.Suppliers.FirstOrDefaultAsync(s => s.SupplierID == dto.SupplierID);
            if (supplier == null) return HttpResponses<ProductResponseDto>.FailResponse("Supplier does not exist.");

            string imagePath = product.ProductImage;

            if (dto.ProductImage != null)
            {
                if (!string.IsNullOrWhiteSpace(product.ProductImage))
                {
                    await FileHelper.DeleteProductImageAsync(product.ProductImage);
                }

                // Save new file
                var imageSavedResult = await FileHelper.SaveProductImageAsync(dto.ProductImage, cancellationToken);
                if (!imageSavedResult.Success)
                {
                    return HttpResponses<ProductResponseDto>.FailResponse(imageSavedResult.Message);
                }

                imagePath = imageSavedResult.Data;
            }

            var baseUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var imageUrl = !string.IsNullOrWhiteSpace(imagePath) ? $"{baseUrl}{imagePath}" : "";

            product.ProductName = dto.ProductName;
            product.Category = dto.Category;
            product.Price = dto.Price;
            product.StockQuantity = dto.StockQuantity;
            product.SupplierID = dto.SupplierID;
            product.IsActive = dto.IsActive;
            product.ProductImage = imageUrl;

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
                IsActive = product.IsActive,
                ProductImage = product.ProductImage
            };

            return HttpResponses<ProductResponseDto>.SuccessResponse(response, "Product updated successfully.");
        }

        public HttpResponses<string> TryValidateRow(IXLRow row)
        {
            string errorMessage;

            var productName = row.Cell(1).GetValue<string>();
            if (string.IsNullOrWhiteSpace(productName))
            {
                errorMessage = $"Row {row.RowNumber()}: Product name is empty.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var category = row.Cell(2).GetValue<string>();
            if (string.IsNullOrWhiteSpace(category))
            {
                errorMessage = $"Row {row.RowNumber()}: Category is empty.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var priceStr = row.Cell(3).GetValue<string>();
            if (!decimal.TryParse(priceStr, out var price) || price <= 0)
            {
                errorMessage = $"Row {row.RowNumber()}: Invalid price '{priceStr}'. Price must be greater than 0.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var qtyStr = row.Cell(4).GetValue<string>();
            if (!int.TryParse(qtyStr, out var qty) || qty < 0)
            {
                errorMessage = $"Row {row.RowNumber()}: Invalid stock qty '{qtyStr}'. Quantity must be ≥ 0.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var supplierStr = row.Cell(5).GetValue<string>();
            if (!int.TryParse(supplierStr, out var supplierId) || supplierId <= 0)
            {
                errorMessage = $"Row {row.RowNumber()}: Invalid supplier ID '{supplierStr}'. Supplier ID must be > 0.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var activeStr = row.Cell(6).GetValue<string>();
            if (!bool.TryParse(activeStr, out _))
            {
                errorMessage = $"Row {row.RowNumber()}: Invalid active flag '{activeStr}'. Must be TRUE or FALSE.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            var urlStr = row.Cell(7).GetValue<string>();
            if (string.IsNullOrWhiteSpace(urlStr))
            {
                errorMessage = $"Row {row.RowNumber()}: Product image URL is empty.";
                return new HttpResponses<string>()
                {
                    Success = false,
                    Message = errorMessage
                };
            }

            return new HttpResponses<string>()
            {
                Success = true,
                Message = "Validation successful"
            };
        }

        public async Task<HttpResponses<ProductReviewResponseDto>> CreateAsync(CreateProductReviewDto dto, string userId, CancellationToken cancellationToken)
        {
            var productExists = await _context.Products.AnyAsync(p => p.ProductID == dto.ProductId, cancellationToken);

            if (!productExists) return HttpResponses<ProductReviewResponseDto>.FailResponse("Product does not exist.");
            var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync() ?? new User();

            var review = new ProductReview
            {
                ProductId = dto.ProductId,
                Comment = dto.Comment,
                Rating = dto.Rating,
                ReviewerName = user.FullName ?? "",
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProductReviews.Add(review);
            await _context.SaveChangesAsync(cancellationToken);

            var response = MapReview(review);
            return HttpResponses<ProductReviewResponseDto>.SuccessResponse(response, "Review added successfully.");
        }

        public async Task<HttpResponses<ProductReviewResponseDto>> UpdateAsync(UpdateProductReviewDto dto, string userId, CancellationToken cancellationToken)
        {
            var review = await _context.ProductReviews.FirstOrDefaultAsync(r => r.ReviewId == dto.ReviewId && r.UserId == userId, cancellationToken);

            if (review == null) return HttpResponses<ProductReviewResponseDto>.FailResponse("Review not found.");

            review.Comment = dto.Comment;
            review.Rating = dto.Rating;

            _context.ProductReviews.Update(review);
            await _context.SaveChangesAsync(cancellationToken);

            var response = MapReview(review);
            return HttpResponses<ProductReviewResponseDto>.SuccessResponse(response, "Review updated successfully.");
        }

        public async Task<HttpResponses<string>> DeleteAsync(int reviewId, CancellationToken cancellationToken)
        {
            var review = await _context.ProductReviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);

            if (review == null) return HttpResponses<string>.FailResponse("Review not found.");

            _context.ProductReviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return HttpResponses<string>.SuccessResponse("Review deleted successfully.");
        }

        public async Task<HttpResponses<ProductReviewResponseDto>> GetByIdAsync(int reviewId, CancellationToken cancellationToken)
        {
            var review = await _context.ProductReviews.FirstOrDefaultAsync(r => r.ReviewId == reviewId, cancellationToken);

            if (review == null) return HttpResponses<ProductReviewResponseDto>.FailResponse("Review not found.");

            var response = MapReview(review);
            return HttpResponses<ProductReviewResponseDto>.SuccessResponse(response);
        }

        public async Task<HttpResponses<List<ProductReviewResponseDto>>> GetByProductIdAsync(int productId, CancellationToken cancellationToken)
        {
            var reviews = await _context.ProductReviews.Where(r => r.ProductId == productId).OrderByDescending(r => r.CreatedAt).ToListAsync(cancellationToken);

            var mapped = reviews.Select(MapReview).ToList();

            return HttpResponses<List<ProductReviewResponseDto>>.SuccessResponse(mapped, "Product reviews retrieved.");
        }

        private ProductReviewResponseDto MapReview(ProductReview review)
        {
            return new ProductReviewResponseDto
            {
                ReviewId = review.ReviewId,
                ProductId = review.ProductId,
                ReviewerName = review.ReviewerName,
                Comment = review.Comment,
                Rating = review.Rating,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
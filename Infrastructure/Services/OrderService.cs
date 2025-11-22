using System.Data;
using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;
        private readonly IUtlityServices _utlityServices;

        public OrderService(AppDbContext context, IUtlityServices utlityServices)
        {
            _context = context;
            _utlityServices = utlityServices;
        }

        public async Task<HttpResponses<OrderResponseDto>> CreateOrderAsync(OrderCreateDto dto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, cancellationToken);
            try
            {
                //  lock timeout to 30 seconds
                await _context.Database.ExecuteSqlRawAsync("SET LOCK_TIMEOUT 10000", cancellationToken);

                var productIds = dto.OrderDetails.Select(d => d.ProductID).Distinct().OrderBy(id => id).ToList();

                //lock products for update
                var products = await _context.Products.FromSqlInterpolated($@"
                    SELECT * FROM Products WITH (UPDLOCK, ROWLOCK)
                    WHERE ProductID IN ({string.Join(",", productIds)}) 
                    ORDER BY ProductID").ToListAsync(cancellationToken);


                var missingProductIds = productIds.Except(products.Select(p => p.ProductID)).ToList();
                if (missingProductIds.Any())
                {
                    return HttpResponses<OrderResponseDto>.FailResponse($"Products not found: {string.Join(", ", missingProductIds)}");
                }

                // validations and stock update
                var orderDetails = new List<OrderDetails>();

                foreach (var item in dto.OrderDetails)
                {
                    var product = products.First(p => p.ProductID == item.ProductID);

                    if (product.StockQuantity < item.Quantity)
                    {
                        return HttpResponses<OrderResponseDto>.FailResponse($"Insufficient stock for product '{product.ProductName}'. Available: {product.StockQuantity}, Requested: {item.Quantity}");
                    }

                    product.StockQuantity -= item.Quantity;

                    orderDetails.Add(new OrderDetails
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        SubTotal = product.Price * item.Quantity
                    });
                }

                var order = new Order
                {
                    OrderDate = dto.OrderDate,
                    CustomerName = dto.CustomerName,
                    CustomerEmail = dto.CustomerEmail,
                    TotalAmount = orderDetails.Sum(od => od.SubTotal),
                    OrderStatus = OrderStatus.Pending.ToString(),
                    OrderDetails = orderDetails
                };

                _context.Orders.Add(order);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var mapped = MapToDto(order);
                return HttpResponses<OrderResponseDto>.SuccessResponse(mapped, "Order created successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return HandleOrderException(ex);
            }
        }

        public async Task<HttpResponses<OrderResponseDto>> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderID == orderId, cancellationToken);

            if (order == null) return HttpResponses<OrderResponseDto>.FailResponse("Order not found.");

            var mapped = MapToDto(order);
            return HttpResponses<OrderResponseDto>.SuccessResponse(mapped, "Order retrieved successfully.");
        }

        public async Task<HttpResponses<PagedResult<OrderResponseDto>>> GetOrdersAsync(GetOrdersDto dto, CancellationToken cancellationToken)
        {
            var query = _context.Orders.Include(o => o.OrderDetails).ThenInclude(od => od.Product).AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.Search))
            {
                var s = dto.Search.ToLower();
                query = query.Where(o => o.CustomerName.ToLower().Contains(s) || o.CustomerEmail.ToLower().Contains(s));
            }

            if (!string.IsNullOrEmpty(dto.Status)) query = query.Where(o => o.OrderStatus == dto.Status);

            if (dto.StartDate.HasValue) query = query.Where(o => o.OrderDate >= dto.StartDate.Value);

            if (dto.EndDate.HasValue) query = query.Where(o => o.OrderDate <= dto.EndDate.Value);

            var totalCount = await query.CountAsync(cancellationToken);
            //
             var orders = await query.OrderByDescending(o => o.OrderDate).Skip((dto.PageNumber - 1) * dto.PageSize).Take(dto.PageSize).ToListAsync(cancellationToken);
             if (orders.Count == 0)
             {
                 return HttpResponses<PagedResult<OrderResponseDto>>.SuccessResponse(null, "No orders found.");
             }

             List<OrderResponseDto> orderList = new List<OrderResponseDto>();
             foreach (var order in orders)
             {
                 var mappedOrder = MapToDto(order);
                 orderList.Add(mappedOrder);
             }
             
             var result = new PagedResult<OrderResponseDto>
            {
                Items = orderList,
                TotalCount = totalCount,
                 PageNumber = dto.PageNumber,
                PageSize = dto.PageSize
            };

            return HttpResponses<PagedResult<OrderResponseDto>>.SuccessResponse(result, "Orders retrieved successfully.");
        }

        public async Task<HttpResponses<OrderResponseDto>> UpdateOrderAsync(int orderId, OrderUpdateDto dto, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderID == orderId, cancellationToken);

            if (order == null) return HttpResponses<OrderResponseDto>.FailResponse("Order not found.");

            order.OrderDate = dto.OrderDate;
            order.CustomerName = dto.CustomerName;
            order.CustomerEmail = dto.CustomerEmail;
            order.TotalAmount = dto.TotalAmount;
            order.OrderStatus = dto.OrderStatus;

            _context.OrderDetails.RemoveRange(order.OrderDetails);

            order.OrderDetails = dto.OrderDetails.Select(d => new OrderDetails
            {
                ProductID = d.ProductID,
                Quantity = d.Quantity,
                SubTotal = d.SubTotal
            }).ToList();

            await _context.SaveChangesAsync(cancellationToken);

            var mapped = MapToDto(order);
            return HttpResponses<OrderResponseDto>.SuccessResponse(mapped, "Order updated successfully.");
        }

        public async Task<byte[]> ExportOrderData(GetOrdersDto dto, CancellationToken cancellationToken)
        {
            var getOrdersResponse = await GetOrdersAsync(dto, cancellationToken);
            var finalData = CleanAndSanitizeData(getOrdersResponse.Data.Items);
            var utilityResult = _utlityServices.ExportToExcel(finalData, "orderExcelSheet");
            return utilityResult;
        }

        public async Task<byte[]> ExportOrderDataPdf(GetOrdersDto dto, CancellationToken cancellationToken)
        {
            var getOrdersResponse = await GetOrdersAsync(dto, cancellationToken);
            var finalData = CleanAndSanitizeData(getOrdersResponse.Data.Items);
            var utilityResult = _utlityServices.ConvertHtmlToPdf(finalData, "OrderReport.html");
            return utilityResult;
        }

        public async Task<HttpResponses<string>> DeleteOrderAsync(int orderId, CancellationToken cancellationToken)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderID == orderId, cancellationToken);

            if (order == null) return HttpResponses<string>.FailResponse("Order not found.");

            _context.OrderDetails.RemoveRange(order.OrderDetails);
            _context.Orders.Remove(order);

            await _context.SaveChangesAsync(cancellationToken);

            return HttpResponses<string>.SuccessResponse(null, "Order deleted successfully.");
        }

        private static OrderResponseDto MapToDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderID = order.OrderID,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                CustomerEmail = order.CustomerEmail,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                OrderDetails = order.OrderDetails.Select(d => new OrderDetailResponseDto
                {
                    OrderDetailID = d.OrderDetailID,
                    ProductID = d.ProductID,
                    Quantity = d.Quantity,
                    SubTotal = d.SubTotal,
                    ProductPrice = d.Product.Price,
                    ProductName= d.Product.ProductName
                }).ToList()
            };
        }

        private static HttpResponses<OrderResponseDto> HandleOrderException(Exception ex)
        {
            if (ex is OperationCanceledException) return HttpResponses<OrderResponseDto>.FailResponse("Order creation was cancelled.");

            if (ex is DbUpdateConcurrencyException) return HttpResponses<OrderResponseDto>.FailResponse("Order was modified by another process. Try again.");

            if (ex is DbUpdateException dbEx) return HttpResponses<OrderResponseDto>.FailResponse($"Database update error: {dbEx.InnerException?.Message ?? dbEx.Message}");

            if (ex is SqlException sqlEx)
            {
                return sqlEx.Number switch
                {
                    1222 => HttpResponses<OrderResponseDto>.FailResponse("Order timeout. Please try again."),
                    1205 => HttpResponses<OrderResponseDto>.FailResponse("Order conflict (deadlock). Try again."),
                    547 or 2627 or 2601 => HttpResponses<OrderResponseDto>.FailResponse($"Data integrity error: {sqlEx.Message}"),
                    _ => HttpResponses<OrderResponseDto>.FailResponse($"Database error (Code {sqlEx.Number}): {sqlEx.Message}")
                };
            }

            return HttpResponses<OrderResponseDto>.FailResponse($"Failed to create order: {ex.Message}");
        }
        private List<ExportOrderReportDto> CleanAndSanitizeData(List<OrderResponseDto> dataItems)
        {
            var reportData = new List<ExportOrderReportDto>();

            foreach (var order in dataItems)
            {
                foreach (var detail in order.OrderDetails)
                {
                    reportData.Add(new ExportOrderReportDto
                    {
                        OrderID = order.OrderID,
                        OrderDate = order.OrderDate,
                        CustomerName = order.CustomerName,
                        CustomerEmail = order.CustomerEmail,
                        OrderStatus = order.OrderStatus,
                        TotalAmount = order.TotalAmount,

                        OrderDetailID = detail.OrderDetailID,
                        ProductID = detail.ProductID,
                        ProductName = detail.ProductName ?? "",  
                        Quantity = detail.Quantity,
                        ProductPrice = detail.ProductPrice,
                        SubTotal = detail.SubTotal
                    });
                }
            }

            return reportData;
        }

    }
}
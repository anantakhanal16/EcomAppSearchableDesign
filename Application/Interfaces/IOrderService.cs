using Application.Dtos;
using Application.Helpers;


namespace Application.Interfaces
{
    public interface IOrderService
    {
        Task<HttpResponses<OrderResponseDto>> CreateOrderAsync(OrderCreateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<OrderResponseDto>> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken);
        Task<HttpResponses<PagedResult<OrderResponseDto>>> GetOrdersAsync(GetOrdersDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<OrderResponseDto>> UpdateOrderAsync(int orderId, OrderUpdateDto dto, CancellationToken cancellationToken);
        Task<HttpResponses<string>> DeleteOrderAsync(int orderId, CancellationToken cancellationToken);
        Task<byte[]> ExportOrderData(GetOrdersDto dto, CancellationToken cancellationToken);
        Task<byte[]> ExportOrderDataPdf(GetOrdersDto dto, CancellationToken cancellationToken);
    }
}
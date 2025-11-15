using Application.Dtos;
using Application.Helpers;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcomAppSearchableDesign.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost("create-order")]
    public async Task<HttpResponses<OrderResponseDto>> CreateOrder([FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
    {
        return await orderService.CreateOrderAsync(dto, cancellationToken);
    }

    [HttpGet("get-order/{id:int}")]
    public async Task<HttpResponses<OrderResponseDto>> GetOrder(int id, CancellationToken cancellationToken)
    {
        return await orderService.GetOrderByIdAsync(id, cancellationToken);
    }

    [HttpPost("get-orders")]
    public async Task<HttpResponses<PagedResult<OrderResponseDto>>> GetOrders([FromBody] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        return await orderService.GetOrdersAsync(dto, cancellationToken);
    }

    [HttpPut("update-order/{id:int}")]
    public async Task<HttpResponses<OrderResponseDto>> UpdateOrder(int id, [FromBody] OrderUpdateDto dto, CancellationToken cancellationToken)
    {
        return await orderService.UpdateOrderAsync(id, dto, cancellationToken);
    }

    [HttpDelete("delete-order/{id:int}")]
    public async Task<HttpResponses<string>> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        return await orderService.DeleteOrderAsync(id, cancellationToken);
    }

    [HttpGet("exportDataInExcel")]
    public async Task<IActionResult> ExportOrders([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        var excelBytes = await orderService.ExportOrderData(dto, cancellationToken);
        return File(fileContents: excelBytes, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName: "Orders.xlsx");
    }
    [HttpGet("exportDataInPdf")]
    public async Task<FileContentResult> ExportOrdersDataPdf([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        var pdfBytes= await orderService.ExportOrderDataPdf(dto, cancellationToken);
        return File(pdfBytes, "application/pdf", "OrdersReport.pdf");
    }
}

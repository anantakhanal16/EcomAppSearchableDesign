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
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost("create-order")]
    public async Task<HttpResponses<OrderResponseDto>> CreateOrder([FromBody] OrderCreateDto dto, CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<OrderResponseDto>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;


        return await orderService.CreateOrderAsync(dto, userId, cancellationToken);
    }
    [HttpGet("get-order/{id:int}")]
    public async Task<HttpResponses<OrderResponseDto>> GetOrderbyId(int id, CancellationToken cancellationToken)
    {

        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<OrderResponseDto>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return await orderService.GetOrderByIdAsync(id, userId, cancellationToken);
    }

    [HttpGet("get-user-orders")]
    public async Task<HttpResponses<PagedResult<OrderResponseDto>>> GetUserOrders([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<PagedResult<OrderResponseDto>>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        return await orderService.GetUserOrder(dto, userId, cancellationToken);
    }
    [HttpPut("update-order/{id:int}")]
    public async Task<HttpResponses<OrderResponseDto>> UpdateOrder(int id, [FromBody] OrderUpdateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<OrderResponseDto>();
        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return await orderService.UpdateOrderAsync(id, dto, userId ,cancellationToken);
    }
   
    [HttpDelete("delete-order/{id:int}")]
    public async Task<HttpResponses<string>> DeleteOrder(int id, CancellationToken cancellationToken)
    {
        if (id == 0)
        {
            return ModelState.ToErrorResponse<string>();

        }
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        return await orderService.DeleteOrderAsync(id, userId,cancellationToken);
    }

    [HttpGet("exportDataInExcel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportOrders([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        var excelBytes = await orderService.ExportOrderData(dto, cancellationToken);
        return File(fileContents: excelBytes, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileDownloadName: "Orders.xlsx");
    }
   
    [HttpGet("exportDataInPdf")]
    [Authorize(Roles = "Admin")]
    public async Task<FileContentResult> ExportOrdersDataPdf([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        var pdfBytes = await orderService.ExportOrderDataPdf(dto, cancellationToken);
        return File(pdfBytes, "application/pdf", "OrdersReport.pdf");
    }
    
    [HttpGet("get-all-orders")]
    [Authorize(Roles = "Admin")]
    public async Task<HttpResponses<PagedResult<OrderResponseDto>>> GetOrders([FromQuery] GetOrdersDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ModelState.ToErrorResponse<PagedResult<OrderResponseDto>>();
        }
        return await orderService.GetOrdersAsync(dto, cancellationToken);
    }


}

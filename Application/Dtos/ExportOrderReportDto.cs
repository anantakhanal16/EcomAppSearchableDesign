namespace Application.Dtos;

public class ExportOrderReportDto
{
    public int OrderID { get; set; }
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public string OrderStatus { get; set; }
    public decimal TotalAmount { get; set; }

    // Order detail fields
    public int OrderDetailID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } // Include for report
    public int Quantity { get; set; }
    public decimal ProductPrice { get; set; }
    public decimal SubTotal { get; set; }
}
using Application.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Html2pdf;
using iText.Layout.Properties;
using ClosedXML.Excel;

namespace Infrastructure.Services
{
    public class UtlitesServices : IUtlityServices
    {
        public byte[] ExportToExcel<T>(List<T> data, string sheetName)
        {
            if (data == null || data.Count == 0)
            {
                return Array.Empty<byte>();
            }

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add(sheetName);

                var properties = typeof(T).GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cell(1, i + 1).Value = properties[i].Name;
                }

                for (int row = 0; row < data.Count; row++)
                {
                    for (int col = 0; col < properties.Length; col++)
                    {
                        var value = properties[col].GetValue(data[row]).ToString();
                        worksheet.Cell(row + 2, col + 1).Value = (XLCellValue)value;
                    }
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }

        public byte[] ConvertHtmlToPdf<T>(List<T> data, string templateFile)
        {
            if (data == null || !data.Any()) return Array.Empty<byte>();

            var htmlTemplate = LoadTemplate(templateFile);
            htmlTemplate = htmlTemplate.Replace("{{Title}}", "OrderReport");

            var properties = typeof(T).GetProperties();

            // Dynamically create table headers
            var headersHtml = string.Join("", properties.Select(p => $"<th>{p.Name}</th>"));
            htmlTemplate = htmlTemplate.Replace("{{Headers}}", headersHtml);

            // Dynamically create table rows
            var rowsHtml = string.Join("", data.Select(item =>
            {
                var cellsHtml = string.Join("", properties.Select(p =>
                {
                    var val = p.GetValue(item);
                    string text = val switch
                    {
                        null => "",
                        decimal d => d.ToString("F2"),
                        DateTime dt => dt.ToString("yyyy-MM-dd"),
                        _ => val.ToString()
                    };
                    return $"<td>{text}</td>";
                }));
                return $"<tr>{cellsHtml}</tr>";
            }));

            htmlTemplate = htmlTemplate.Replace("{{Rows}}", rowsHtml);

            // Generate PDF
            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdfDoc = new PdfDocument(writer);

            var converterProperties = new ConverterProperties();
            converterProperties.SetBaseUri(Path.Combine(AppContext.BaseDirectory, "Exports", "Templates"));

            HtmlConverter.ConvertToPdf(htmlTemplate, pdfDoc, converterProperties);

            return stream.ToArray();
        }

        public string LoadTemplate(string templateName)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Exports", "Templates", templateName);
            if (!File.Exists(path)) throw new FileNotFoundException($"Template '{templateName}' not found at {path}");
            return File.ReadAllText(path);
        }
    }
}
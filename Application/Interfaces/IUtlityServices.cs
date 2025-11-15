using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IUtlityServices
    {
        byte[] ExportToExcel<T>(List<T> data, string sheetName);

        byte[] ConvertHtmlToPdf<T>(List<T> data, string templateFile);
    }
}
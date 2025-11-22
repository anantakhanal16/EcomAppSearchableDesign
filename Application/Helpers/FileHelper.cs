using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public class FileHelper
    {
        public static async Task<HttpResponses<string>> SaveProductImageAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null) 
            {
                return HttpResponses<string>.FailResponse("No file provided.");
            }
     
            if (file.Length > 4 * 1024 * 1024) 
            {
                return HttpResponses<string>.FailResponse("Image must be less than 4MB.");
            }
            var folder = Path.Combine("wwwroot", "product-images");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            return HttpResponses<string>.SuccessResponse($"/product-images/{fileName}.");
        }

        public static async Task<HttpResponses<string>> DeleteProductImageAsync(string imageUrlOrPath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imageUrlOrPath))
                    return HttpResponses<string>.FailResponse("Invalid image path.");

                string relativePath = imageUrlOrPath;

                if (imageUrlOrPath.StartsWith("http"))
                {
                    var uri = new Uri(imageUrlOrPath);
                    relativePath = uri.AbsolutePath; 
                }

                string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string fullPath = Path.Combine(rootPath, relativePath.TrimStart('/'));

                if (!File.Exists(fullPath))
                    return HttpResponses<string>.FailResponse("Old image not found.");

                await Task.Run(() => File.Delete(fullPath));

                return HttpResponses<string>.SuccessResponse("Image deleted successfully.");
            }
            catch (Exception ex)
            {
                return HttpResponses<string>.FailResponse($"Failed to delete old image. {ex.Message}");
            }
        }

    }
}

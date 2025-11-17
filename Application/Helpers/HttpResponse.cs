using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Application.Helpers
{
    public class HttpResponses<T> : IActionResult where T : class
    {
        public HttpStatusCode StatusCode { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }

        public HttpResponses() { }

        public HttpResponses(bool success, string message, T data, HttpStatusCode statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
        }

        public static HttpResponses<T> SuccessResponse(
            T data,
            string message = "Request successful",
            HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new HttpResponses<T>
            {
                StatusCode = statusCode,
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static HttpResponses<T> FailResponse(
            string message,
            object errors = null,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new HttpResponses<T>
            {
                StatusCode = statusCode,
                Success = false,
                Message = message,
                Errors = errors,
                Data = default
            };
        }

        public static HttpResponses<T> ErrorResponse(
            string message = "An error occurred",
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            return new HttpResponses<T>
            {
                StatusCode = statusCode,
                Success = false,
                Message = message,
                Data = default
            };
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(this)
            {
                StatusCode = (int)StatusCode
            };
            await objectResult.ExecuteResultAsync(context);
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Linq;

namespace Application.Helpers
{
    public static class ModelStateExtensions
    {
        public static HttpResponses<T> ToErrorResponse<T>(this ModelStateDictionary modelState)
            where T : class
        {
            var errors = modelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Error = e.Value.Errors.First().ErrorMessage??"Error Occured"
                })
                .ToList();

            return HttpResponses<T>.FailResponse("Validation failed",errors);
        }
    }
}

namespace Application.Helpers
{
    public class ServiceResponseData<T> : ServiceResponse
    {
        public T Data { get; set; }

        public static ServiceResponseData<T> Success(string message = "Operation successful", T data = default!)
        {
            return new ServiceResponseData<T> { Code = "0", Message = message, Data = data };
        }

        public static ServiceResponseData<T> Failure(string message = "Operation failed")
        {
            return new ServiceResponseData<T> { Code = "1", Message = message, Data = default };
        }
    }
}
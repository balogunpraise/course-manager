namespace Core.Application.Dtos.Responses
{
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public BaseResponse()
        {
            Errors = new List<string>();
        }

        public BaseResponse(int statusCode, bool succeeded, string message, List<string> errors = null)
        {
            StatusCode = statusCode;
            Succeeded = succeeded;
            Message = message;
            Errors = errors ?? new List<string>();
        }


        public static BaseResponse Success(int statusCode, string message)
        {
            return new BaseResponse(statusCode, true, message);
        }

        public static BaseResponse Failure(int statusCode = 400, string message = "Request Failed", List<string> errors = null)
        {
            var response = new BaseResponse(statusCode, false, message, errors);
            return response;
        }
    }

    public class BaseResponse<T> : BaseResponse
    {
        public T Data { get; set; }
        public BaseResponse() : base()
        {
        }
        public BaseResponse(int statusCode, bool succeeded, string message, T data = default, List<string> errors = null)
            : base(statusCode, succeeded, message, errors)
        {
            Data = data;
        }
        public static BaseResponse<T> Success(int statusCode, string message, T data)
        {
            return new BaseResponse<T>(statusCode, true, message, data);
        }
        public new static BaseResponse<T> Failure(int statusCode = 400, string message = "Request Failed", List<string> errors = null)
        {
            var response = new BaseResponse<T>(statusCode, false, message, default(T), errors);
            return response;
        }
    }
}

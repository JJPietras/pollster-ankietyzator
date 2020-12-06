using System.Net;

namespace Ankietyzator.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public HttpStatusCode Code { get; set; }
        
        public ServiceResponse<T> Failure(string message, HttpStatusCode code)
        {
            Message = message;
            Code = code;
            return this;
        }
        
        public ServiceResponse<T> Failure<TK>(ServiceResponse<TK> response)
        {
            Message = response.Message;
            Code = response.Code;
            return this;
        }

        public ServiceResponse<T> Success(T data, string message)
        {
            Data = data;
            Message = message;
            Code = HttpStatusCode.OK;
            return this;
        }
    }
}
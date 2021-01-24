namespace Ankietyzator.Models
{
    public class Response<T>
    {
        public T Data { get; }
        public string Message { get; }
        
        public Response(ServiceResponse<T> serviceResponse)
        {
            Data = serviceResponse.Data;
            Message = serviceResponse.Message;
        }
    }
}
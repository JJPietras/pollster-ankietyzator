namespace Ankietyzator.Models
{
    public class Response<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        
        public Response<T> Failure(string message)
        {
            Message = message;
            return this;
        }

        public Response<T> Success(T data, string message)
        {
            Data = data;
            Message = message;
            return this;
        }
    }
}
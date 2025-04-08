namespace OnlineMovieTicket.BL.DTOs
{

    public class Response
    {
        public bool Success { get; set; }
        public string? Message { get; set; }

        public Response(bool success, string? message)
        {
            Success = success;
            Message = message;
        }
    }
    public class Response<T> : Response
    {
        public T? Data {get; set;}
        public Response(bool success, string? message, T? data = default) : base(success, message)
        {
            Data = data;
        }
    }
}

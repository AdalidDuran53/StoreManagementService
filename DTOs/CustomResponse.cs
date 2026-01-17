using System;

namespace DTOs
{
    public class CustomResponse
    {
        public int StatusCode { get; }
        public string Message { get; }
        public Guid ClientId { get; }
        public Guid? SessionId { get; }
        public object Data { get; }

        public CustomResponse(int statusCode, string message, Guid clientId, Guid? sessionId = null, object data = null)
        {
            StatusCode = statusCode;
            this.Message = message;
            this.ClientId = clientId;
            this.SessionId = sessionId;
            this.Data = data;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public class ErrorResponse
    {
        public ErrorResponse() { }

        public ErrorResponse(int statusCode, string code, string message, string? details)
        {
            StatusCode = statusCode;
            Code = code;
            Message = message;
            Details = details;
        }
        public int StatusCode { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}

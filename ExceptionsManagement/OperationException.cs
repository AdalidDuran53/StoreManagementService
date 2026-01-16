using System;
using System.Collections.Generic;
using System.Text;

namespace ExceptionsManagement
{
    public class OperationException : Exception
    {
        public OperationException() { }
        public OperationException(string errorCode, string message, string details, Guid? sessionId)
        {
            this.ErrorCode = errorCode;
            this.Message = message;
            this.Details = details;
            this.SessionId = sessionId;
        }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public Guid? SessionId { get; set; }
    }
}

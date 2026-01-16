using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace DTOs
{
    public partial class OperationLog : IValidation
    {
        public OperationLog()
        {
        }

        public OperationLog(DateTime operationDate, string request, string response, Guid? sessionId = null)
        {
            this.SessionId = sessionId;
            this.OperationDate = operationDate;
            this.Request = request;
            this.Response = response;
        }
        public int OperationId { get; set; }

        public Guid? SessionId { get; set; }

        public DateTime? OperationDate { get; set; }

        public string? Request { get; set; }

        public string? Response { get; set; }

        public virtual SessionLog? Session { get; set; }

        public string Validate(string operationExceptionCode)
        {
            throw new NotImplementedException();
        }
    }
}

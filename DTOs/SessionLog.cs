using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public partial class SessionLog : IValidation
    {
        public SessionLog()
        {
        }

        public SessionLog(Guid sessionId, Guid? userId, DateTime initSession)
        {
            this.SessionId = sessionId;
            this.UserId = userId;
            this.InitSession = initSession;
        }
        public Guid SessionId { get; set; }

        public Guid? UserId { get; set; }

        public DateTime InitSession { get; set; }

        public DateTime? EndSession { get; set; }

        public virtual User? User { get; set; }

        public string Validate(string operationExceptionCode)
        {
            throw new NotImplementedException();
        }
    }
}

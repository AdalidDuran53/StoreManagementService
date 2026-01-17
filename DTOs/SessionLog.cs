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

        public SessionLog(Guid sessionId, Guid? clientId, DateTime initSession)
        {
            this.SessionId = sessionId;
            this.ClientId = clientId;
            this.InitSession = initSession;
        }
        public Guid SessionId { get; set; }

        public Guid? ClientId { get; set; }

        public DateTime InitSession { get; set; }

        public DateTime? EndSession { get; set; }

        public virtual Client? User { get; set; }

        public string Validate(string operationExceptionCode)
        {
            throw new NotImplementedException();
        }
    }
}

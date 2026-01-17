using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class SessionLog
    {
        public SessionLog()
        {
            OperationLogs = new HashSet<OperationLog>();
        }

        public Guid SessionId { get; set; }
        public Guid? ClientId { get; set; }
        public DateTime InitSession { get; set; }
        public DateTime? EndSession { get; set; }

        public virtual Client Client { get; set; }
        public virtual ICollection<OperationLog> OperationLogs { get; set; }
    }
}

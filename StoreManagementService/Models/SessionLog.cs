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
        public Guid? UserId { get; set; }
        public DateTime InitSession { get; set; }
        public DateTime? EndSession { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OperationLog> OperationLogs { get; set; }
    }
}

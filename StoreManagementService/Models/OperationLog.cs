using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class OperationLog
    {
        public int OperationId { get; set; }
        public Guid? SessionId { get; set; }
        public DateTime? OperationDate { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }

        public virtual SessionLog Session { get; set; }
    }
}

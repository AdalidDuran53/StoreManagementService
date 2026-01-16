using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class ItemsClientsRelationship
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ItemId { get; set; }
        public DateTime OperationDate { get; set; }

        public virtual Client Client { get; set; }
        public virtual Item Item { get; set; }
    }
}

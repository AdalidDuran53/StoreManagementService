using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class ItemsStoresRelationship
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime OperationDate { get; set; }

        public virtual Item Item { get; set; }
        public virtual Store Store { get; set; }
    }
}

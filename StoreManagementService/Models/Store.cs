using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class Store
    {
        public Store()
        {
            ItemsStoresRelationships = new HashSet<ItemsStoresRelationship>();
        }

        public Guid StoreId { get; set; }
        public string StoreBranch { get; set; }
        public string StoreAddress { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<ItemsStoresRelationship> ItemsStoresRelationships { get; set; }
    }
}

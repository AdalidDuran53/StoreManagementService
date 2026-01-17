using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class Item
    {
        public Item()
        {
            ItemsClientsRelationships = new HashSet<ItemsClientsRelationship>();
            ItemsStoresRelationships = new HashSet<ItemsStoresRelationship>();
        }

        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public byte[] ItemImg { get; set; }
        public int ItemStock { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<ItemsClientsRelationship> ItemsClientsRelationships { get; set; }
        public virtual ICollection<ItemsStoresRelationship> ItemsStoresRelationships { get; set; }
    }
}

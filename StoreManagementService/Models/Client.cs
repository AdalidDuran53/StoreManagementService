using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class Client
    {
        public Client()
        {
            ItemsClientsRelationships = new HashSet<ItemsClientsRelationship>();
        }

        public Guid ClientId { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientAddress { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<ItemsClientsRelationship> ItemsClientsRelationships { get; set; }
    }
}

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
            SessionLogs = new HashSet<SessionLog>();
        }

        public Guid ClientId { get; set; }
        public string UserName { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientAddress { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalst { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<ItemsClientsRelationship> ItemsClientsRelationships { get; set; }
        public virtual ICollection<SessionLog> SessionLogs { get; set; }
    }
}

using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class User
    {
        public User()
        {
            SessionLogs = new HashSet<SessionLog>();
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalst { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<SessionLog> SessionLogs { get; set; }
    }
}

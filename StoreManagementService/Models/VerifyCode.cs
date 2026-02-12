using System;
using System.Collections.Generic;

#nullable disable

namespace StoreManagementService.Models
{
    public partial class VerifyCode
    {
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? Token { get; set; }
        public int? VerifyStatus { get; set; }
        public string Code { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? VerifyDate { get; set; }

        public virtual Client Client { get; set; }
    }
}

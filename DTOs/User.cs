using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public partial class User : IValidation
    {
        public User()
        {
        }

        public User(Guid userId, string userName, string password, string salst)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.PasswordHash = password;
            this.PasswordSalst = salst;
        }
        public Guid UserId { get; set; }

        public string? UserName { get; set; }

        public string PasswordHash { get; set; } = null!;
        public string PasswordSalst { get; set; } = null!;

        public bool? IsDeleted { get; set; }

        // Validate the user object
        public string Validate(string operationExceptionCode)
        {
            if (String.IsNullOrEmpty(this.UserName))
                operationExceptionCode = "OMS-USERNAME-ERROR";
            else if (this.UserName.Length > 50)
                operationExceptionCode = "OMS-USERNAME-ERROR";
            else if (String.IsNullOrEmpty(this.PasswordHash))
                operationExceptionCode = "OMS-PASSWORD-ERROR";
            else if (this.PasswordHash.Length > 50)
                operationExceptionCode = "OMS-PASSWORD-ERROR";

            return operationExceptionCode;
        }

    }
}

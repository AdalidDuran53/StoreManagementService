using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public partial class Client : IValidation
    {
        public Client()
        {
        }

        public Client(Guid clientId, string emailAddress, string clientName, string clientLastName, string clientAddress, string password, string salst)
        {
            this.ClientId = clientId;
            this.EmailAddress = emailAddress;
            this.ClientName = clientName;
            this.ClientLastName = clientLastName;
            this.ClientAddress = clientAddress;
            this.PasswordHash = password;
            this.PasswordSalst = salst;
        }

        public Guid ClientId { get; set; }
        public string EmailAddress { get; set; }
        public string ClientName { get; set; }
        public string ClientLastName { get; set; }
        public string ClientAddress { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalst { get; set; }
        public bool? isLogin { get; set; }
        public bool? IsDeleted { get; set; }


        // Validate the user object
        public string Validate(string operationExceptionCode)
        {
            if (String.IsNullOrEmpty(this.EmailAddress))
                operationExceptionCode = "OMS-USERNAME-ERROR";
            else if (this.EmailAddress.Length > 50)
                operationExceptionCode = "OMS-USERNAME-ERROR";
            else if (String.IsNullOrEmpty(this.PasswordHash))
                operationExceptionCode = "OMS-PASSWORD-ERROR";
            else if (this.PasswordHash.Length > 50)
                operationExceptionCode = "OMS-PASSWORD-ERROR";

            else if(!this.isLogin.GetValueOrDefault()) {
                if (String.IsNullOrEmpty(this.ClientName))
                    operationExceptionCode = "OMS-CLIENTNAME-ERROR";
                else if (String.IsNullOrEmpty(this.ClientLastName))
                    operationExceptionCode = "OMS-CLIENTLASTNAME-ERROR";
                else if (String.IsNullOrEmpty(this.ClientAddress))
                    operationExceptionCode = "OMS-CLIENTADDRESS-ERROR";
                else if (this.ClientName.Length > 50)
                    operationExceptionCode = "OMS-CLIENTNAME-ERROR";
                else if (this.ClientLastName.Length > 100)
                    operationExceptionCode = "OMS-CLIENTLASTNAME-ERROR";
            }

            return operationExceptionCode;
        }

    }
}

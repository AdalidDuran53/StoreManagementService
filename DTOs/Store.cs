using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public class Store : IValidation
    {
        public Store()
        {
        }
        public Store(Guid storeId, string storeBranch, string storeAddress)
        {
            this.StoreId = storeId;
            this.StoreBranch = storeBranch;
            this.StoreAddress = storeAddress;
        }

        public Guid StoreId { get; set; }
        public string StoreBranch { get; set; }
        public string StoreAddress { get; set; }
        public bool? IsDeleted { get; set; }

        public string Validate(string operationExceptionCode)
        {
            if(String.IsNullOrEmpty(this.StoreBranch))
                operationExceptionCode = "OMS-STOREBRANCH-ERROR";
            else if (String.IsNullOrEmpty(this.StoreAddress))
                operationExceptionCode = "OMS-STOREADDRESS-ERROR";
            else if (this.StoreBranch.Length > 50)
                operationExceptionCode = "OMS-STOREBRANCH-ERROR";
            else if (this.StoreAddress.Length > 100)
                operationExceptionCode = "OMS-STOREADDRESS-ERROR";

            return operationExceptionCode;
        }
    }
}

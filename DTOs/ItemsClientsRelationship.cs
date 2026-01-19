using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public class ItemsClientsRelationship : IValidation
    {
        public ItemsClientsRelationship()
        {
        }
        public ItemsClientsRelationship(Guid id, Guid? clientId,  Guid? itemId, int itemAmount, DateTime operationDate)
        {
            this.Id = id;
            this.ClientId = clientId;
            this.ItemId = itemId;
            this.ItemAmount = itemAmount;
            this.OperationDate = operationDate;
        }
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ItemId { get; set; }
        public int ItemAmount { get; set; }
        public DateTime OperationDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? WasSold { get; set; }

        public string Validate(string operationExceptionCode)
        {
            if (this.ItemAmount <= 0)
                operationExceptionCode = "OMS-ITEMAMOUNT-ERROR";

            return operationExceptionCode;
        }
    }
}

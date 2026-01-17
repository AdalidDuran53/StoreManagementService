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
        public ItemsClientsRelationship(Guid id, Guid? clientId,  Guid? itemId, int itemAmont, DateTime operationDate)
        {
            this.Id = id;
            this.ClientId = clientId;
            this.ItemId = itemId;
            this.ItemAmont = itemAmont;
            this.OperationDate = operationDate;
        }
        public Guid Id { get; set; }
        public Guid? ClientId { get; set; }
        public Guid? ItemId { get; set; }
        public int ItemAmont { get; set; }
        public DateTime OperationDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? WasSold { get; set; }

        public string Validate(string operationExceptionCode)
        {
            if (this.ItemAmont <= 0)
                operationExceptionCode = "OMS-ITEMAMOUNT-ERROR";

            return operationExceptionCode;
        }
    }
}

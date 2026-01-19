using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public class ItemsStoresRelationship : IValidation
    {
        public ItemsStoresRelationship()
        {
        }
        public ItemsStoresRelationship(Guid id, Guid? itemId, Guid? storeId, DateTime operationDate)
        {
            this.Id = id;
            this.ItemId = itemId;
            this.StoreId = storeId;
            this.OperationDate = operationDate;
        }
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public Guid? StoreId { get; set; }
        public DateTime OperationDate { get; set; }

        public string Validate(string operationExceptionCode)
        {
            return operationExceptionCode;
        }
    }
}

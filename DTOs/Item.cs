using ExceptionsManagement;
using System;
using System.Collections.Generic;
using System.Text;

namespace DTOs
{
    public class Item : IValidation
    {
        public Item()
        {
        }

        public Item(Guid itemId, string itemCode, string itemDescription, decimal itemPrice, byte[] itemImg, int itemStock)
        {
            this.ItemId = itemId;
            this.ItemCode = itemCode;
            this.ItemDescription = itemDescription;
            this.ItemPrice = itemPrice;
            this.ItemImg = itemImg;
            this.ItemStock = itemStock;
        }

        public Guid ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public decimal ItemPrice { get; set; }
        public byte[] ItemImg { get; set; }
        public int ItemStock { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsUpdate { get; set; }

        public string Validate(string operationExceptionCode)
        {
            if (this.ItemCode.Length > 50)
                operationExceptionCode = "OMS-ITEMCODE-ERROR";
            else if (this.ItemDescription.Length > 50)
                operationExceptionCode = "OMS-ITEMDESCRIPTION-ERROR";
            else if (!this.IsUpdate.GetValueOrDefault())
            {
                if (string.IsNullOrEmpty(this.ItemCode))
                    operationExceptionCode = "OMS-ITEMCODE-ERROR";
                else if (string.IsNullOrEmpty(this.ItemDescription))
                    operationExceptionCode = "OMS-ITEMDESCRIPTION-ERROR";
            }

            return operationExceptionCode;
        }
    }
}

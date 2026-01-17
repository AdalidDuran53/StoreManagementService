using DTOs;
using ExceptionsManagement;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class ItemFunctionality : FunctionalityBaseController
    {
        public async Task<ActionResult> addItem(Guid clientId, Guid sessionId, string itemCode, string itemDescription, decimal itemPrice, IFormFile itemImg, int itemStock)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);

                // Convert IFormFile to byte[]
                byte[] itemImgBytes = null;
                if (itemImg != null && itemImg.Length > 0)
                {
                    using (var ms = new System.IO.MemoryStream())
                    {
                        await itemImg.CopyToAsync(ms);
                        itemImgBytes = ms.ToArray();
                    }
                }

                // build the item object
                Item newItem = new Item(itemId: Guid.NewGuid(), itemCode: itemCode, itemDescription: itemDescription, itemPrice: itemPrice, itemImg: itemImgBytes, itemStock: itemStock);
                // validate the item object
                this.ValidateModel(newItem);
                // save the item object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // map the item object to the entity model
                    var item = Mapster.TypeAdapter.Adapt<Models.Item>(newItem);
                    context.Items.Add(item);
                    await context.SaveChangesAsync();
                }
                // return the result
                var result = Ok(new { success = true, message = "Data saved successfully." });
                return result;
            }
            catch (Exception ex)
            {
                // if the exception is an OperationException, rethrow it
                if (ex is OperationException)
                    throw ex;
                // otherwise, throw a general error
                var exception = this._errorService.GetError("OMS-GENERAL-ERROR");
                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
            }
        }

        public async Task<CustomResponse> GetItem(Guid clientId, Guid sessionId, Guid? itemId)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // init the list of item
                    List<Models.Item> existingItems = new List<Models.Item>();
                    // if itemId has value
                    if (itemId.HasValue)
                    {
                        // check if the item exists
                        var item = await context.Items.FirstOrDefaultAsync(t => t.ItemId == itemId && t.IsDeleted == false);
                        // if not, throw an exception
                        if (item == null)
                        {
                            var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                            throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                        }
                        else
                            existingItems.Add(item);
                    }
                    else
                    {
                        // get all items
                        existingItems = await context.Items.Where(t => t.IsDeleted == false).ToListAsync();
                    }

                    var itemsResult = existingItems.Adapt<List<Item>>();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Get data successfully.", clientId: clientId, sessionId: sessionId, data: itemsResult);
                }

            }
            catch (Exception ex)
            {
                // if the exception is an OperationException, rethrow it
                if (ex is OperationException)
                    throw ex;
                // otherwise, throw a general error
                var exception = this._errorService.GetError("OMS-GENERAL-ERROR");
                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
            }
        }

        public async Task<CustomResponse> UpdateItem(Guid clientId, Guid sessionId, Guid itemId, string itemCode, string itemDescription, decimal? itemPrice, IFormFile? itemImg, int? itemStock)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the item by StoreId
                    var item = await context.Items
                    .FirstOrDefaultAsync(u => u.ItemId == itemId && u.IsDeleted == false);

                    if (!String.IsNullOrEmpty(itemCode))
                        item.ItemCode = itemCode;
                    if (!String.IsNullOrEmpty(itemDescription))
                        item.ItemDescription = itemDescription;
                    if(itemPrice.HasValue)
                        item.ItemPrice = itemPrice.Value;
                    if(itemStock.HasValue)
                        item.ItemStock = itemStock.Value;
                    if (itemImg != null && itemImg.Length > 0)
                    {
                        using (var ms = new System.IO.MemoryStream())
                        {
                            await itemImg.CopyToAsync(ms);
                            item.ItemImg = ms.ToArray();
                        }
                    }

                    // map the store object to custom store model for validation
                    var updatedItem = Mapster.TypeAdapter.Adapt<Item>(item);
                    this.ValidateModel(updatedItem);
                    // update the store
                    context.Items.Update(item);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Updated item successfully.", clientId: clientId, sessionId: sessionId);


                }
            }
            catch (Exception ex)
            {
                // if the exception is an OperationException, rethrow it
                if (ex is OperationException)
                    throw ex;
                // otherwise, throw a general error
                var exception = this._errorService.GetError("OMS-GENERAL-ERROR");
                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
            }
        }

        public async Task<CustomResponse> DeleteItem(Guid clientId, Guid sessionId, Guid itemId)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the Item by StoreId
                    var item = await context.Items
                    .FirstOrDefaultAsync(u => u.ItemId == itemId && u.IsDeleted == false);
                    if (item == null)
                    {
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
                    }

                    // mark the Item as deleted
                    item.IsDeleted = true;
                    context.Items.Update(item);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Deleted item successfully.", clientId: clientId, sessionId: sessionId);
                }
            }
            catch (Exception ex)
            {
                // if the exception is an OperationException, rethrow it
                if (ex is OperationException)
                    throw ex;
                // otherwise, throw a general error
                var exception = this._errorService.GetError("OMS-GENERAL-ERROR");
                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
            }
        }
    }
}

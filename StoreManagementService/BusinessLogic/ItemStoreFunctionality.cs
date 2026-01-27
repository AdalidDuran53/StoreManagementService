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
using System.Net;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class ItemStoreFunctionality : FunctionalityBaseController
    {
        public async Task<ActionResult> addItem(Guid clientId, Guid sessionId, Guid itemID, Guid storeId, DateTime operationDate)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);

                // build the item object
                ItemsStoresRelationship newItem = new ItemsStoresRelationship(id: Guid.NewGuid(), itemId: itemID, storeId: storeId, operationDate: operationDate);
                // validate the item object
                this.ValidateModel(newItem);
                // save the item object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {

                    // check if the item is no deleted
                    var itemStock = await context.Items.FirstOrDefaultAsync(s => s.ItemId.Equals(newItem.ItemId) && s.IsDeleted == false);
                    if (itemStock == null)
                    {
                        // if not, throw an exception
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    // map the item object to the entity model
                    var item = Mapster.TypeAdapter.Adapt<Models.ItemsStoresRelationship>(newItem);
                    context.ItemsStoresRelationships.Add(item);
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

        public async Task<CustomResponse> GetItem(Guid clientId, Guid sessionId, Guid storeId, Guid? itemId)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // init the list of item
                    List<Models.ItemsStoresRelationship> existingItems = new List<Models.ItemsStoresRelationship>();
                    // if itemId has value
                    if (itemId.HasValue)
                    {
                        // check if the item exists
                        var item = await context.ItemsStoresRelationships.FirstOrDefaultAsync(t => t.StoreId == storeId && t.ItemId == itemId && (t.IsDeleted == false || t.IsDeleted == null));
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
                        existingItems = await context.ItemsStoresRelationships.Where(i => i.StoreId == storeId && (i.IsDeleted == false || i.IsDeleted == null)).ToListAsync();
                    }

                    var itemsResult = existingItems.Adapt<List<ItemsStoresRelationship>>();
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

        public async Task<CustomResponse> DeleteItem(Guid clientId, Guid sessionId, Guid storeId, Guid itemId)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the Item by StoreId
                    var item = await context.ItemsStoresRelationships
                    .FirstOrDefaultAsync(u => u.StoreId == storeId && u.ItemId == itemId && u.IsDeleted == false);
                    if (item == null)
                    {
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
                    }

                    // mark the Item as deleted
                    item.IsDeleted = true;
                    context.ItemsStoresRelationships.Update(item);
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

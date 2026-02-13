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
    public class ItemClientFunctionality : FunctionalityBaseController
    {
        public async Task<CustomResponse> addItem(Guid clientId, Guid sessionId, Guid itemID, int itemAmount, DateTime operationDate)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);

                // build the item object
                ItemsClientsRelationship newItem = new ItemsClientsRelationship(id: Guid.NewGuid(), clientId: clientId, itemId: itemID, itemAmount: itemAmount, operationDate: operationDate);
                // validate the item object
                this.ValidateModel(newItem);
                // save the item object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {

                    // check if the item stock is sufficient
                    var itemStock = await context.Items.FirstOrDefaultAsync(s => s.ItemId.Equals(newItem.ItemId));
                    if (itemStock == null)
                    {
                        // if is no sufficient, throw an exception
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }
                    if (itemStock.ItemStock < newItem.ItemAmount)
                    {
                        // if is no sufficient, throw an exception
                        var exception = this._errorService.GetError("OMS-ITEMAMOUNT-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    // map the item object to the entity model
                    var item = Mapster.TypeAdapter.Adapt<Models.ItemsClientsRelationship>(newItem);
                    context.ItemsClientsRelationships.Add(item);
                    await context.SaveChangesAsync();
                }
                // return the result
                return new CustomResponse(statusCode: StatusCodes.Status201Created, message: "added item successfully.", clientId: clientId, sessionId: sessionId);
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
                    List<Models.ItemsClientsRelationship> existingItems = new List<Models.ItemsClientsRelationship>();
                    // if itemId has value
                    if (itemId.HasValue)
                    {
                        // check if the item exists
                        var item = await context.ItemsClientsRelationships.FirstOrDefaultAsync(t => t.ItemId == itemId && t.IsDeleted == false && t.WasSold == false && t.ClientId == clientId);
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
                        existingItems = await context.ItemsClientsRelationships.Where(t => t.IsDeleted == false && t.WasSold == false && t.ClientId == clientId).ToListAsync();
                    }

                    var itemsResult = existingItems.Adapt<List<ItemsClientsRelationship>>();
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

        public async Task<CustomResponse> SellItems(Guid clientId, Guid sessionId)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {// init the list of item
                    List<Models.ItemsClientsRelationship> existingItems = new List<Models.ItemsClientsRelationship>();
                    // check if the item exists
                    var items = await context.ItemsClientsRelationships.Where(t => t.ClientId == clientId && t.IsDeleted == false && t.WasSold == false).ToListAsync();
                    var itemsIds = items.Select(i => i.ItemId).ToList();
                    var itemsStock = await context.Items.Where(i => itemsIds.Contains(i.ItemId)).ToListAsync();
                    // if not, throw an exception
                    if (items == null || items.Count == 0)
                    {
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    foreach (var item in items)
                    {
                        // check if the stock is sufficient
                        var itemStock = itemsStock.FirstOrDefault(i => i.ItemId == item.ItemId);
                        if (itemStock.ItemStock >= item.ItemAmount && itemStock.IsDeleted == false)
                        {
                            // mark the item as sold
                            item.WasSold = true;
                            context.ItemsClientsRelationships.Update(item);
                            // decrease the stock
                            itemStock.ItemStock -= item.ItemAmount;
                            context.Items.Update(itemStock);
                        } // check if the item was deleted
                        else if(itemStock.IsDeleted == true)
                        {
                            item.IsDeleted = true;
                            context.ItemsClientsRelationships.Update(item);
                        }
                    }
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
                    var item = await context.ItemsClientsRelationships
                    .FirstOrDefaultAsync(u => u.ItemId == itemId && u.IsDeleted == false && u.WasSold == false && u.ClientId == clientId);
                    if (item == null)
                    {
                        var exception = this._errorService.GetError("OMS-ITEM-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
                    }

                    // mark the Item as deleted
                    item.IsDeleted = true;
                    context.ItemsClientsRelationships.Update(item);
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

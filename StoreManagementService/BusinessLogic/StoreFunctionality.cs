using DTOs;
using ExceptionsManagement;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class StoreFunctionality : FunctionalityBaseController
    {
        public async Task<ActionResult> addStore(Guid clientId, Guid sessionId, string storeBranch, string storeAddress)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                // build the store object
                Store newStore = new Store(storeId: Guid.NewGuid(), storeBranch: storeBranch, storeAddress: storeAddress);
                // validate the store object
                this.ValidateModel(newStore);
                // save the store object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // map the store object to the entity model
                    var store = Mapster.TypeAdapter.Adapt<Models.Store>(newStore);
                    context.Stores.Add(store);
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

        public async Task<CustomResponse> GetStore(Guid clientId, Guid sessionId, Guid? storeId)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // init the list of stores
                    List<Models.Store> existingTransactions = new List<Models.Store>();
                    // if storeId has value
                    if (storeId.HasValue)
                    {
                        // check if the store exists
                        var transaction = await context.Stores.FirstOrDefaultAsync(t => t.StoreId == storeId && t.IsDeleted == false);
                        // if not, throw an exception
                        if (transaction == null)
                        {
                            var exception = this._errorService.GetError("OMS-TRANSACTION-NOT-FOUND");
                            throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                        }
                        else
                            existingTransactions.Add(transaction);
                    }
                    else
                    {
                        // get all stores
                        existingTransactions = await context.Stores.Where(t => t.IsDeleted == false).ToListAsync();
                    }

                    var transactionsResult = existingTransactions.Adapt<List<Store>>();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Get data successfully.", clientId: clientId, sessionId: sessionId, data: transactionsResult);
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

        public async Task<CustomResponse> UpdateStore(Guid clientId, Guid sessionId, Guid storeId, string newStoreBranch, string newStoreAddress)
        {
            try
            {
                // validate the session
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the Store by StoreId
                    var store = await context.Stores
                    .FirstOrDefaultAsync(u => u.StoreId == storeId && u.IsDeleted == false);

                    // if new Store Branch is provided, hash it and update the Branch
                    if (!String.IsNullOrEmpty(newStoreBranch))
                        store.StoreBranch = newStoreBranch;
                    // if new Store Address is provided, update the Address
                    if (!String.IsNullOrEmpty(newStoreAddress))
                        store.StoreAddress = newStoreAddress;

                    // map the store object to custom store model for validation
                    var updatedStore = Mapster.TypeAdapter.Adapt<Store>(store);
                    this.ValidateModel(updatedStore);
                    // update the store
                    context.Stores.Update(store);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Updated user successfully.", clientId: clientId, sessionId: sessionId);


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

        public async Task<CustomResponse> DeleteStore(Guid clientId, Guid sessionId, Guid storeId)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the Store by StoreId
                    var store = await context.Stores
                    .FirstOrDefaultAsync(u => u.StoreId == storeId && u.IsDeleted == false);
                    if (store == null)
                    {
                        var exception = this._errorService.GetError("OMS-STORE-NOTFOUND-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, sessionId: sessionId);
                    }

                    // mark the Store as deleted
                    store.IsDeleted = true;
                    context.Stores.Update(store);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Deleted store successfully.", clientId: clientId, sessionId: sessionId);
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

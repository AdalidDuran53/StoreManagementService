using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagementService.BusinessLogic;
using StoreManagementService.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StoreManagementService.Controllers.Implementation
{
    [ApiVersion("0.1")]
    [ApiController]
    public class ItemsStoresRelationshipImplementationController : ItemsStoresRelationshipControllerBase
    {
        private readonly ItemStoreFunctionality _itemStoreFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public ItemsStoresRelationshipImplementationController(ItemStoreFunctionality itemFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _itemStoreFunctionality = itemFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/ItemsStore/AddItem")]
        public async override Task<IActionResult> AddItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid itemID, [Required] Guid storeId, [Required] DateTime operationDate)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewItemsStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemID: " + itemID, "storeId: " + storeId, "operationDate: " + operationDate } } };
            try
            {
                // Call the implementation
                var result = await _itemStoreFunctionality.addItem(clientId: clientId, sessionId: sessionId, itemID: itemID, storeId: storeId, operationDate: operationDate);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "CreateNewItemsStoreResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorCreateItemsStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/ItemsStore/GetItem")]
        public async override Task<IActionResult> GetItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid storeId, Guid? itemId = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "GetItemsStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemId: " + itemId } } };
            try
            {
                // log the operation
                var operationId = await _serviceBaseFunctionality.LogOperation(request, new Dictionary<string, object>(), sessionId);
                // Call the implementation
                var result = await _itemStoreFunctionality.GetItem(clientId, sessionId, storeId, itemId);
                // Update the operation log
                await _serviceBaseFunctionality.UpdateOperation((int)operationId.Data, request, new Dictionary<string, object> { { "GetItemsStoreRequest", result } }, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId, data: result.Data));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorGetItemsStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/ItemsStore/DeleteItem")]
        public async override Task<IActionResult> DeleteItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid storeId, [Required] Guid itemId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteItemsStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _itemStoreFunctionality.DeleteItem(clientId, sessionId, storeId, itemId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteItemsStoreResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteItemsStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }
    }
}

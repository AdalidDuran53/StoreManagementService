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
    public class ItemsClientsRelationshipImplementationController : ItemsClientsRelationshipControllerBase
    {
        private readonly ItemClientFunctionality _itemClientFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public ItemsClientsRelationshipImplementationController(ItemClientFunctionality itemFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _itemClientFunctionality = itemFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/ItemsClient/AddItem")]
        public async override Task<IActionResult> AddItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid itemID, [Required] int itemAmount, [Required] DateTime operationDate)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewItemsClientClientRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemID: " + itemID, "itemAmount: " + itemAmount, "operationDate: " + operationDate } } };
            try
            {
                // Call the implementation
                var result = await _itemClientFunctionality.addItem(clientId: clientId, sessionId: sessionId, itemID: itemID, itemAmount: itemAmount, operationDate: operationDate);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "CreateNewItemsClientClientResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorCreateNewItemsClientClientResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/ItemsClient/GetItem")]
        public async override Task<IActionResult> GetItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid? itemId = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "GetItemsClientRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemId: " + itemId } } };
            try
            {
                // log the operation
                var operationId = await _serviceBaseFunctionality.LogOperation(request, new Dictionary<string, object>(), sessionId);
                // Call the implementation
                var result = await _itemClientFunctionality.GetItem(clientId, sessionId, itemId);
                // Update the operation log
                await _serviceBaseFunctionality.UpdateOperation((int)operationId.Data, request, new Dictionary<string, object> { { "GetItemsClientRequest", result } }, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId, data: result.Data));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorGetItemsClientResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPut]
        [Route("~/{version::apiVersion}/ItemsClient/SellItem")]
        public async override Task<IActionResult> SellItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "SellItemsClientRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId} } };
            try
            {
                // Call the implementation
                var result = await _itemClientFunctionality.SellItems(clientId, sessionId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "SellItemsClientResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorSellItemsClientResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/ItemsClient/DeleteItem")]
        public async override Task<IActionResult> DeleteItem([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid itemId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteItemsClientRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _itemClientFunctionality.DeleteItem(clientId, sessionId, itemId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteItemsClientResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteItemsClientResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }
    }
}

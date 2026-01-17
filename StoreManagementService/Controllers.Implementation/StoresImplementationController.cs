using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagementService.BusinessLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StoreManagementService.Controllers.Implementation
{
    [ApiVersion("0.1")]
    [ApiController]
    public class StoresImplementationController : StoresControllerBase
    {
        private readonly StoreFunctionality _storeFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public StoresImplementationController(StoreFunctionality storeFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _storeFunctionality = storeFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Stores/AddStore")]
        public async override Task<IActionResult> AddStore([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] string storeBranch, [Required] string storeAddress)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "storeBranch: " + storeBranch, "storeAddress: " + storeAddress } } };
            try
            {
                // Call the implementation
                var result = await _storeFunctionality.addStore(clientId, sessionId, storeBranch, storeAddress);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "CreateNewStoreResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorCreateNewStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpGet]
        [Route("~/{version::apiVersion}/Stores/GetStore")]
        public async override Task<IActionResult> GetStore([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid? storeId = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "GetStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "transactionId: " + storeId } } };
            try
            {
                // log the operation
                var operationId = await _serviceBaseFunctionality.LogOperation(request, new Dictionary<string, object>(), sessionId);
                // Call the implementation
                var result = await _storeFunctionality.GetStore(clientId, sessionId, storeId);
                // Update the operation log
                await _serviceBaseFunctionality.UpdateOperation((int)operationId.Data, request, new Dictionary<string, object> { { "GetStoreRequest", result } }, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId, data: result.Data));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorGetStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPut]
        [Route("~/{version::apiVersion}/Stores/UpdateStore")]
        public async override Task<IActionResult> UpdateStore([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid storeId, string newStoreBranch = null, string newStoreAddress = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "UpdateStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "storeId: " + storeId, "newStoreBranch: " + newStoreBranch, "newStoreAddress: " + newStoreAddress } } };
            try
            {
                // Call the implementation
                var result = await _storeFunctionality.UpdateStore(clientId, sessionId, storeId, newStoreBranch, newStoreAddress);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "UpdateStoreResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorUpdateStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/Stores/DeleteStore")]
        public async override Task<IActionResult> DeleteStore([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid storeId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _storeFunctionality.DeleteStore(clientId, sessionId, storeId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteStoreResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteStoreResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }
    }
}

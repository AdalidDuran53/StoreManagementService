using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagementService.BusinessLogic;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static StoreManagementService.FilterProvider.ExamplesProvider;

namespace StoreManagementService.Controllers.Implementation
{
    [ApiVersion("0.1")]
    [ApiController]
    [EnableCors("AllowAll")]
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
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CustomResponseCreatedExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status201Created, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> AddStore(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            [Required] string storeBranch,
            [Required] string storeAddress)
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
                return this.Created(String.Empty, result);
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
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> GetStore(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId,
            [Required] Guid sessionId, 
            Guid? storeId = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "GetStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "storeId: " + storeId } } };
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
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> UpdateStore(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            [Required] Guid storeId, 
            string newStoreBranch = null,
            string newStoreAddress = null)
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
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> DeleteStore(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId,
            [Required] Guid sessionId, 
            Guid storeId)
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

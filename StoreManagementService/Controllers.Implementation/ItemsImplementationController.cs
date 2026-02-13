using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreManagementService.BusinessLogic;
using StoreManagementService.Models;
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
    public class ItemsImplementationController : ItemsControllerBase
    {
        private readonly ItemFunctionality _itemFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public ItemsImplementationController(ItemFunctionality itemFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _itemFunctionality = itemFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Items/AddItem")]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CustomResponseCreatedExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status201Created, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> AddItem(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId,
            [Required] Guid sessionId,
            [Required] string itemCode, 
            [Required] string itemDescription,
            [Required] decimal itemPrice,
            [Required] IFormFile itemImg,
            [Required] int itemStock)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewStoreRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _itemFunctionality.addItem(clientId, sessionId, itemCode, itemDescription, itemPrice, itemImg, itemStock);
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
        [Route("~/{version::apiVersion}/Items/GetItem")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> GetItem(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            Guid? itemId = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "GetItemRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemId: " + itemId } } };
            try
            {
                // log the operation
                var operationId = await _serviceBaseFunctionality.LogOperation(request, new Dictionary<string, object>(), sessionId);
                // Call the implementation
                var result = await _itemFunctionality.GetItem(clientId, sessionId, itemId);
                // Update the operation log
                await _serviceBaseFunctionality.UpdateOperation((int)operationId.Data, request, new Dictionary<string, object> { { "GetItemRequest", result } }, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId, data: result.Data));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorGetItemResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPut]
        [Route("~/{version::apiVersion}/Items/UpdateItem")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> UpdateItem(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version, 
            [Required] Guid clientId, 
            [Required] Guid sessionId,
            [Required] Guid itemId, 
            string itemCode = null, 
            string itemDescription = null, 
            decimal? itemPrice = null, 
            IFormFile itemImg = null,
            int? itemStock = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "UpdateItemRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "itemId: " + itemId, "itemCode: " + itemCode, "itemDescription: " + itemDescription, "itemPrice: " + itemPrice, "itemImg: " + (itemImg != null), "itemStock: " + itemStock } } };
            try
            {
                // Call the implementation
                var result = await _itemFunctionality.UpdateItem(clientId, sessionId, itemId, itemCode, itemDescription, itemPrice, itemImg, itemStock);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "UpdateItemResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorUpdateItemResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/Items/DeleteItem")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        public async override Task<IActionResult> DeleteItem(
            [FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required][DefaultValue("0.1")] string version,
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            Guid itemId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteItemRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _itemFunctionality.DeleteItem(clientId, sessionId, itemId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteItemResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteItemResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }
    }
}

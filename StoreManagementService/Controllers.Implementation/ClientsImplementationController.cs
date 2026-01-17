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
    public class ClientsImplementationController : ClientsControllerBase
    {
        private readonly ClientFunctionality _clientFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public ClientsImplementationController(ClientFunctionality clientFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _clientFunctionality = clientFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Clients/AddClient")]
        public override async Task<IActionResult> AddClient([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password, [Required] string clientName, [Required] string clientLastName, [Required] string clientAddress)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewUserRequest", new object[] { "version: " + version, "userName: " + userName } } };
            try
            {
                // Call the implementation
                var result = await _clientFunctionality.AddClient(userName, clientName, clientLastName, clientAddress, password);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "CreateNewUserResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorCreateNewUserResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Clients/Login")]
        public async override Task<IActionResult> Login([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "LoginRequest", new object[] { "version: " + version, "userName: " + userName } } };
            try
            {
                // Call the implementation
                var result = await _clientFunctionality.LoginUser(userName, password);
                // Log the response
                var sessionLogResponse = await _serviceBaseFunctionality.InitSession(result.ClientId);
                Dictionary<string, object> response = new Dictionary<string, object> { { "LoginResponse", result }, { "SessionLogResponse", sessionLogResponse } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result // OMS-13 update to include session id in the response
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionLogResponse.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorLoginResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPut]
        [Route("~/{version::apiVersion}/Clients/UpdateClient")]
        public async override Task<IActionResult> UpdateClient([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] string currentPassword, string newPassword = null, string userName = null, string newClientName = null, string newClientLastName = null, string newClientAddress = null)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "UpdateUserRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId, "newPassword: " + !String.IsNullOrEmpty(newPassword), "newUserName: " + !String.IsNullOrEmpty(userName), "newClientName: " + !String.IsNullOrEmpty(newClientName), "newClientLastName: " + !String.IsNullOrEmpty(newClientLastName), "newClientAddress: " + !String.IsNullOrEmpty(newClientAddress) } } };
            try
            {
                // Call the implementation
                var result = await _clientFunctionality.UpdateUser(clientId, sessionId, currentPassword, newPassword, userName, newClientName, newClientLastName, newClientAddress);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "UpdateUserResponse", result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorUpdateUserResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/Clients/DeleteUser")]
        public async override Task<IActionResult> DeleteClient([FromRoute, RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$"), Required] string version, [Required] Guid clientId, [Required] Guid sessionId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteClientRequest", new object[] { "version: " + version, "clientId: " + clientId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _clientFunctionality.DeleteClient(clientId, sessionId);
                // close all sessions for the user
                var sessionLogResponse = _serviceBaseFunctionality.CloseAllSession(clientId, sessionId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteClientResponse", result }, { "SessionLogResponse", sessionLogResponse.Result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, clientId: result.ClientId, sessionId: sessionLogResponse.Result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteClientResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }
    }
}

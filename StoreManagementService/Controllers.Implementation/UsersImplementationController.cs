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
    public class UsersImplementationController : UsersControllerBase
    {
        private readonly UserFunctionality _userFunctionality;
        private readonly ServiceBaseFunctionality _serviceBaseFunctionality;
        private readonly ErrorServiceModel _errorService = new ErrorServiceModel();
        public UsersImplementationController(UserFunctionality userFunctionality, ServiceBaseFunctionality serviceBaseFunctionality)
        {
            _userFunctionality = userFunctionality;
            _serviceBaseFunctionality = serviceBaseFunctionality;
            _errorService = new ErrorServiceModel();
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Users/AddUser")]
        public override async Task<IActionResult> AddUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "CreateNewUserRequest", new object[] { "version: " + version, "userName: " + userName } } };
            try
            {
                // Call the implementation
                var result = await _userFunctionality.AddUser(userName, password);
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
        [Route("~/{version::apiVersion}/Users/Login")]
        public async override Task<IActionResult> Login([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "LoginRequest", new object[] { "version: " + version, "userName: " + userName } } };
            try
            {
                // Call the implementation
                var result = await _userFunctionality.LoginUser(userName, password);
                // Log the response
                var sessionLogResponse = await _serviceBaseFunctionality.InitSession(result.UserId);
                Dictionary<string, object> response = new Dictionary<string, object> { { "LoginResponse", result }, { "SessionLogResponse", sessionLogResponse } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response);
                // return the result // OMS-13 update to include session id in the response
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, userId: result.UserId, sessionId: sessionLogResponse.SessionId));
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

        [HttpDelete]
        [Route("~/{version::apiVersion}/Users/DeleteUser")]
        public async override Task<IActionResult> DeleteUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] Guid userId, [Required] Guid sessionId)
        {
            // Log the request
            Dictionary<string, object> request = new Dictionary<string, object> { { "DeleteUserRequest", new object[] { "version: " + version, "userId: " + userId, "sessionId: " + sessionId } } };
            try
            {
                // Call the implementation
                var result = await _userFunctionality.DeleteUser(userId, sessionId);
                // close all sessions for the user
                var sessionLogResponse = _serviceBaseFunctionality.CloseAllSession(userId, sessionId);
                // Log the response
                Dictionary<string, object> response = new Dictionary<string, object> { { "DeleteUserResponse", result }, { "SessionLogResponse", sessionLogResponse.Result } };
                // Log the operation
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // return the result
                return Ok(new CustomResponse(statusCode: StatusCodes.Status200OK, message: result.Message, userId: result.UserId, sessionId: sessionLogResponse.Result.SessionId));
            }
            catch (Exception ex)
            {
                // Log the exception
                Dictionary<string, object> response = new Dictionary<string, object> { { "ErrorDeleteUserResponse", ex } };
                await _serviceBaseFunctionality.LogOperation(request, response, sessionId);
                // if the exception is an OperationException, return a bad request with the error details
                OperationException excep = ((OperationException)ex);
                return this.BadRequest(new { StatusCode = StatusCodes.Status400BadRequest, code = excep.ErrorCode, message = excep.Message, details = excep.Details });
            }
        }

        [HttpPut]
        [Route("~/{version::apiVersion}/Users/UpdateUser")]
        public override Task<IActionResult> UpdateUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] Guid userId, [Required] Guid sessionId, [Required] string currentPassword, string newPassword = null, string userName = null)
        {
            return Task.FromResult<IActionResult>(Ok("AddUser"));
        }
    }
}

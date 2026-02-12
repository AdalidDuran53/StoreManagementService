using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using StoreManagementService.Models;
using Client = DTOs.Client;

namespace StoreManagementService.BusinessLogic
{
    public class EmailVerifyFunctionality : FunctionalityBaseController
    {

        public HttpClient GetHtpClient()
        {
            //Declaramos un cliente
            HttpClient httpClient = new HttpClient();
            string AuthenticationKey = Environment.GetEnvironmentVariable("AuthenticationKey");
            httpClient.DefaultRequestHeaders.Add("AuthenticationKey", AuthenticationKey);
            httpClient.Timeout = TimeSpan.FromMinutes(5);
            return httpClient;
        }
        public async Task<CustomResponse> RequestVerifyCode(string emailAddress, Guid clientId)
        {
            try
            {
                HttpClient client = GetHtpClient();

                string EVSBaseUrl = Environment.GetEnvironmentVariable("EVSBaseUrl");
                string EVSVersion = Environment.GetEnvironmentVariable("EVSVersion");
                Guid appToken = new Guid(Environment.GetEnvironmentVariable("appToken"));
                Guid AuthenticationKey = new Guid(Environment.GetEnvironmentVariable("AuthenticationKey"));
                
                EVS.EmailVerifyClient emailVerifyClient = new EVS.EmailVerifyClient(EVSBaseUrl, client);
                
                var result = await emailVerifyClient.RequestVerifyCodeAsync(EVSVersion, emailAddress, appToken, AuthenticationKey);

                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    VerifyCode verifyCode = new VerifyCode();
                    verifyCode.Token = result.Token;
                    verifyCode.VerifyStatus = (int)VerifyStatusCodes.pending;
                    verifyCode.ClientId = clientId;
                    verifyCode.CreationDate = DateTime.Now;
                    context.VerifyCodes.Add(verifyCode);
                }

                return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "send Verification Code successfully", clientId: clientId);
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

        public async Task<CustomResponse> LoginUser(string emailAddress, string password)
        {
            try
            {
                // hash the password
                var pass = HashPassword(password);
                // build the user object
                Client dataClient = new Client(clientId: Guid.NewGuid(), emailAddress: emailAddress, clientName: string.Empty, clientLastName: string.Empty, clientAddress: string.Empty, password: pass.Hash, salst: pass.Salt);
                dataClient.isLogin = true;
                // validate the user object
                this.ValidateModel(dataClient);
                // check the user credentials
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var client = await context.Clients
                    .FirstOrDefaultAsync(u => u.EmailAddress == dataClient.EmailAddress && u.IsDeleted == false);
                    // if the user is not found or the password does not match, throw an error
                    if (client == null || !this.VerifyPassword(password, client.PasswordHash, client.PasswordSalst))
                    {
                        var exception = this._errorService.GetError("OMS-LOGIN-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "validated verify code successfully.", clientId: client.ClientId);
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

        public async Task<CustomResponse> DeleteClient(Guid clientId, Guid sessionId)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var client = await context.Clients
                    .FirstOrDefaultAsync(u => u.ClientId == clientId && u.IsDeleted == false);
                    // mark the user as deleted
                    client.IsDeleted = true;
                    context.Clients.Update(client);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Deleted user successfully.", clientId: client.ClientId, sessionId: sessionId);
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

        public async Task<CustomResponse> UpdateUser(Guid clientId, Guid sessionId, string currentPassword, string newPassword, string userName, string newClientName, string newClientLastName, string newClientAddress)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var user = await context.Clients
                    .FirstOrDefaultAsync(u => u.ClientId == clientId && u.IsDeleted == false);
                    // if the user is not found, the session id does not match or the current password does not match, throw an error
                    if (!VerifyPassword(currentPassword, user.PasswordHash, user.PasswordSalst))
                    {
                        var exception = this._errorService.GetError("OMS-SESSION-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }
                    else
                    {
                        // if new password is provided, hash it and update the password
                        if (!String.IsNullOrEmpty(newPassword))
                        {
                            // hash the new password
                            var pass = HashPassword(newPassword);
                            user.PasswordHash = pass.Hash;
                            user.PasswordSalst = pass.Salt;
                        }
                        // if user name is provided, update the user name
                        if (!String.IsNullOrEmpty(userName))
                        {
                            // check for duplicate user names
                            var isInvalidUserName = await context.Clients.AnyAsync(s => s.EmailAddress.Equals(userName) && s.ClientId != clientId);
                            if (isInvalidUserName)
                            {
                                // if the user name already exists, throw an error
                                var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                            }
                            user.EmailAddress = userName;
                        }
                        if (!String.IsNullOrEmpty(newClientName))
                            user.ClientName = newClientName;
                        if (!String.IsNullOrEmpty(newClientLastName))
                            user.ClientLastName = newClientLastName;
                        if (!String.IsNullOrEmpty(newClientAddress))
                            user.ClientAddress = newClientAddress;
                        // map the user object to custom user model for validation
                        var updatedUser = Mapster.TypeAdapter.Adapt<Client>(user);
                        this.ValidateModel(updatedUser);
                        // update the user
                        context.Clients.Update(user);
                        await context.SaveChangesAsync();
                        // return the result
                        return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Updated user successfully.", clientId: user.ClientId, sessionId: sessionId);
                    }

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

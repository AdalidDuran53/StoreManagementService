using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class UserFunctionality : FunctionalityBaseController
    {
        public async Task<ActionResult> AddClient(string userName, string clientName, string clientLastName, string clientAddress, string password)
        {
            try
            {

                var pass = HashPassword(password);
                // build the user object
                Client newClient = new Client(clientId: Guid.NewGuid(), userName: userName, clientName: clientName, clientLastName: clientLastName, clientAddress: clientAddress, password: pass.Hash, salst: pass.Salt);
                // validate the user object
                this.ValidateModel(newClient);
                // save the user object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // check for duplicate user names
                    var isInvalidUserName = await context.Clients.AnyAsync(s => s.UserName.Equals(newClient.UserName));
                    if (isInvalidUserName)
                    {
                        // if the user name already exists, throw an error
                        var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }
                    // map the user object to the entity model
                    var client = Mapster.TypeAdapter.Adapt<Models.Client>(newClient);
                    context.Clients.Add(client);
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

        public async Task<CustomResponse> LoginUser(string userName, string password)
        {
            try
            {
                // hash the password
                var pass = HashPassword(password);
                // build the user object
                Client dataClient = new Client(clientId: Guid.NewGuid(), userName: userName, clientName: string.Empty, clientLastName: string.Empty, clientAddress: string.Empty, password: pass.Hash, salst: pass.Salt);
                dataClient.isLogin = true;
                // validate the user object
                this.ValidateModel(dataClient);
                // check the user credentials
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var client = await context.Clients
                    .FirstOrDefaultAsync(u => u.UserName == dataClient.UserName && u.IsDeleted == false);
                    // if the user is not found or the password does not match, throw an error
                    if (client == null || !this.VerifyPassword(password, client.PasswordHash, client.PasswordSalst))
                    {
                        var exception = this._errorService.GetError("OMS-LOGIN-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Login successfully.", clientId: client.ClientId);
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
                            var isInvalidUserName = await context.Clients.AnyAsync(s => s.UserName.Equals(userName) && s.ClientId != clientId);
                            if (isInvalidUserName)
                            {
                                // if the user name already exists, throw an error
                                var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                            }
                            user.UserName = userName;
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

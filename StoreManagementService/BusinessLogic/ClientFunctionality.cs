using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StoreManagementService.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class ClientFunctionality : FunctionalityBaseController
    {
        public async Task<CustomResponse> AddClient(string emailAddress, string clientName, string clientLastName, string clientAddress, string password)
        {
            try
            {

                var pass = HashPassword(password);
                // build the user object
                DTOs.Client newClient = new DTOs.Client(clientId: Guid.NewGuid(), emailAddress: emailAddress, clientName: clientName, clientLastName: clientLastName, clientAddress: clientAddress, password: pass.Hash, salst: pass.Salt);
                // validate the user object
                this.ValidateModel(newClient);
                // save the user object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // check for duplicate user names
                    var isInvalidUserName = await context.Clients.AnyAsync(s => s.EmailAddress.Equals(newClient.EmailAddress));
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
                return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "added user successfully.", clientId: newClient.ClientId); ;
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
                DTOs.Client dataClient = new DTOs.Client(clientId: Guid.NewGuid(), emailAddress: emailAddress, clientName: string.Empty, clientLastName: string.Empty, clientAddress: string.Empty, password: pass.Hash, salst: pass.Salt);
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

                    var verifyCodes = await context.VerifyCodes.Where(x => x.ClientId == client.ClientId).ToListAsync();

                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Login successfully.", clientId: client.ClientId, data: verifyCodes.Any(s => s.VerifyStatus == (int)VerifyStatusCodes.Verified) ? null : verifyCodes.OrderByDescending(s => s.CreationDate).FirstOrDefault().Token);
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

        public async Task<CustomResponse> UpdateUser(Guid clientId, Guid sessionId, string currentPassword, string newPassword, string emailAddress, string newClientName, string newClientLastName, string newClientAddress)
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
                        if (!String.IsNullOrEmpty(emailAddress))
                        {
                            // check for duplicate user names
                            var isInvalidUserName = await context.Clients.AnyAsync(s => s.EmailAddress.Equals(emailAddress) && s.ClientId != clientId);
                            if (isInvalidUserName)
                            {
                                // if the user name already exists, throw an error
                                var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                            }
                            user.EmailAddress = emailAddress;
                        }
                        if (!String.IsNullOrEmpty(newClientName))
                            user.ClientName = newClientName;
                        if (!String.IsNullOrEmpty(newClientLastName))
                            user.ClientLastName = newClientLastName;
                        if (!String.IsNullOrEmpty(newClientAddress))
                            user.ClientAddress = newClientAddress;
                        // map the user object to custom user model for validation
                        var updatedUser = Mapster.TypeAdapter.Adapt<DTOs.Client>(user);
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

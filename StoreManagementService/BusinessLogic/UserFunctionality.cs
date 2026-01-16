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
        public async Task<ActionResult> AddUser(string userName, string password)
        {
            try
            {

                var pass = HashPassword(password);
                // build the user object
                User newUser = new User(userId: Guid.NewGuid(), userName: userName, password: pass.Hash, salst: pass.Salt);
                // validate the user object
                this.ValidateModel(newUser);
                // save the user object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // check for duplicate user names
                    var isInvalidUserName = await context.Users.AnyAsync(s => s.UserName.Equals(newUser.UserName));
                    if (isInvalidUserName)
                    {
                        // if the user name already exists, throw an error
                        var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }
                    // map the user object to the entity model
                    var user = Mapster.TypeAdapter.Adapt<Models.User>(newUser);
                    context.Users.Add(user);
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
                User DataUser = new User(userId: Guid.NewGuid(), userName: userName, password: pass.Hash, salst: pass.Salt);
                // validate the user object
                this.ValidateModel(DataUser);
                // check the user credentials
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var user = await context.Users
                    .FirstOrDefaultAsync(u => u.UserName == DataUser.UserName && u.IsDeleted == false);
                    // if the user is not found or the password does not match, throw an error
                    if (user == null || !this.VerifyPassword(password, user.PasswordHash, user.PasswordSalst))
                    {
                        var exception = this._errorService.GetError("OMS-LOGIN-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }

                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Login successfully.", userId: user.UserId);
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

        public async Task<CustomResponse> DeleteUser(Guid userId, Guid sessionId)
        {
            try
            {
                await this.ValidateSession(userId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var user = await context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted == false);
                    // mark the user as deleted
                    user.IsDeleted = true;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                    // return the result
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Deleted user successfully.", userId: user.UserId, sessionId: sessionId);
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

        public async Task<CustomResponse> UpdateUser(Guid userId, Guid sessionId, string currentPassword, string newPassword, string userName)
        {
            try
            {
                await this.ValidateSession(userId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // find the user by user name
                    var user = await context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId && u.IsDeleted == false);
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
                            var isInvalidUserName = await context.Users.AnyAsync(s => s.UserName.Equals(userName) && s.UserId != userId);
                            if (isInvalidUserName)
                            {
                                // if the user name already exists, throw an error
                                var exception = this._errorService.GetError("OMS-USERNAME-ERROR");
                                throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                            }
                            user.UserName = userName;
                        }
                        // map the user object to custom user model for validation
                        var updatedUser = Mapster.TypeAdapter.Adapt<User>(user);
                        this.ValidateModel(updatedUser);
                        // update the user
                        context.Users.Update(user);
                        await context.SaveChangesAsync();
                        // return the result
                        return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Updated user successfully.", userId: user.UserId, sessionId: sessionId);
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

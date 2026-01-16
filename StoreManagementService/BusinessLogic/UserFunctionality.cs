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
    }
}

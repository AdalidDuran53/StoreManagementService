using DTOs;
using ExceptionsManagement;
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
    }
}

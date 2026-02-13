using DTOs;
using ExceptionsManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StoreManagementService.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
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
        public async Task<CustomResponse> RequestVerifyCode(string emailAddress, Guid clientId, Guid? sessionId = null)
        {
            try
            {

                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    if (String.IsNullOrEmpty(emailAddress))
                    {
                        await this.ValidateSession(clientId, sessionId.GetValueOrDefault());
                        var user = await context.Clients
                        .FirstOrDefaultAsync(u => u.ClientId == clientId && u.IsDeleted == false);

                        if (user != null)
                            emailAddress = user.EmailAddress;
                    }
                    HttpClient client = GetHtpClient();

                    var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                    string EVSBaseUrl = config["EVSBaseUrl"];
                    string EVSVersion = config["EVSVersion"];
                    Guid appToken = Guid.Parse(config["appToken"] ?? Guid.Empty.ToString());
                    Guid AuthenticationKey = Guid.Parse(config["AuthenticationKey"] ?? Guid.Empty.ToString());

                    EVS.EmailVerifyClient emailVerifyClient = new EVS.EmailVerifyClient(EVSBaseUrl, client);

                    var result = await emailVerifyClient.RequestVerifyCodeAsync(EVSVersion, emailAddress, appToken, AuthenticationKey);
                    VerifyCode verifyCode = new VerifyCode();
                    verifyCode.Id = Guid.NewGuid();
                    verifyCode.Token = result.Token;
                    verifyCode.VerifyStatus = (int)VerifyStatusCodes.pending;
                    verifyCode.ClientId = clientId;
                    verifyCode.CreationDate = DateTime.Now;
                    context.VerifyCodes.Add(verifyCode);
                    context.SaveChanges();

                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "send Verification Code successfully", clientId: clientId, data: verifyCode.Token);
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

        public async Task<CustomResponse> VerifyCode(Guid sessionId, Guid clientId, Guid token, string code)
        {
            try
            {
                await this.ValidateSession(clientId, sessionId);
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    HttpClient client = GetHtpClient();

                    var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json")
                    .Build();

                    var verifyCode = await context.VerifyCodes.FirstOrDefaultAsync(s => s.Token == token);
                    var user = await context.Clients
                    .FirstOrDefaultAsync(u => u.ClientId == clientId && u.IsDeleted == false);
                    if (verifyCode == null)
                    {
                        var exception = this._errorService.GetError("OMS-VERIFY-CODE-ERROR");
                        throw new OperationException(errorCode: exception.Code, message: exception.Message, details: exception.Details, new Guid());
                    }
                    string EVSBaseUrl = config["EVSBaseUrl"];
                    string EVSVersion = config["EVSVersion"];
                    Guid appToken = Guid.Parse(config["appToken"] ?? Guid.Empty.ToString());
                    Guid AuthenticationKey = Guid.Parse(config["AuthenticationKey"] ?? Guid.Empty.ToString());

                    EVS.EmailVerifyClient emailVerifyClient = new EVS.EmailVerifyClient(EVSBaseUrl, client);

                    var result = await emailVerifyClient.ValidateVerifyCodeAsync(EVSVersion, user.EmailAddress, token, code, AuthenticationKey);

                    verifyCode.Code = code;
                    verifyCode.VerifyStatus = (int)VerifyStatusCodes.Verified;
                    verifyCode.VerifyDate = DateTime.Now;
                    context.VerifyCodes.Update(verifyCode);
                    context.SaveChanges();
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
    }
}

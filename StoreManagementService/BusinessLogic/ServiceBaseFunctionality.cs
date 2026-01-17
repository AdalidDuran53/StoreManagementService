using DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoreManagementService.BusinessLogic
{
    public class ServiceBaseFunctionality : FunctionalityBaseController
    {
        internal async Task<CustomResponse> LogOperation(Dictionary<string, object> operationRequest, Dictionary<string, object> operationResponse, Guid? sessionId = null)
        {
            try
            {
                // init request and response strings
                string request = string.Empty;
                string response = string.Empty;
                // serialize the request and response dictionaries
                foreach (var item in operationRequest)
                {
                    request += string.Concat(item.Key, " : ", JsonConvert.SerializeObject(item.Value));
                }
                foreach (var item in operationResponse)
                {
                    response += string.Concat(item.Key, " : ", JsonConvert.SerializeObject(item.Value));
                }
                // create a new operation log object
                OperationLog newLogOperation = new OperationLog(sessionId: sessionId, operationDate: DateTime.Now, request: request, response: response);
                // save the operation log object
                using (var context = new StoreManagementService.Models.StoreManagementContext())
                {
                    // map the operation log object to the entity model
                    var newLog = Mapster.TypeAdapter.Adapt<Models.OperationLog>(newLogOperation);
                    context.OperationLogs.Add(newLog);
                    await context.SaveChangesAsync();
                    return new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Data saved successfully.", clientId: new Guid(), sessionId: sessionId, data: newLog.OperationId);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

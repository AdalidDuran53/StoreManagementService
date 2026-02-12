using DTOs;
using Microsoft.AspNetCore.Http;
using StoreManagementService.Models;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace StoreManagementService.FilterProvider
{
    public class ExamplesProvider
    {
        public class CustomResponseCreatedExample : IExamplesProvider<CustomResponse>
        {
            public CustomResponse GetExamples()
                => new CustomResponse(statusCode: StatusCodes.Status201Created, message: "added user successfully.", clientId: Guid.Empty, sessionId: Guid.Empty);
        }

        public class CustomResponseBadRequestExample : IExamplesProvider<ErrorResponse>
        {
            public ErrorResponse GetExamples()
                => new ErrorResponse { StatusCode = StatusCodes.Status400BadRequest, Code = "OMS-GENERAL-ERROR", Message = "unexpected error.", Details = "An unexpected error has occurred in the service. Please try again later or contact the administrator if the problem persists." };
        }

        public class CustomResponseOKExample : IExamplesProvider<CustomResponse>
        {
            public CustomResponse GetExamples()
                => new CustomResponse(statusCode: StatusCodes.Status200OK, message: "Login successfully.", clientId: Guid.Empty, sessionId: Guid.Empty);
        }
    }
}

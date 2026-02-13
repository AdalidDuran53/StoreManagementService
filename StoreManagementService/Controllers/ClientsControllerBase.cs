using DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static StoreManagementService.FilterProvider.ExamplesProvider;

namespace StoreManagementService.Controllers
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("[controller]")]
    [EnableCors("AllowAll")]
    public abstract class ClientsControllerBase : ControllerBase
    {
        [HttpPost]
        [Route("~/{version}/Clients/")]
        [SwaggerOperation(OperationId = "AddClient")]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CustomResponseCreatedExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status201Created, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> AddClient([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, [Required, EmailAddress] string emailAddress, [Required] string password, [Required] string clientName, [Required] string clientLastName, [Required] string clientAddress);

        [HttpPost]
        [Route("~/{version}/Clients/")]
        [SwaggerOperation(OperationId = "Login")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> Login([FromRoute][Required][RegularExpression(
            "^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required, EmailAddress] string emailAddress, 
            [Required] string password);

        [HttpPut]
        [Route("~/{version}/Clients/")]
        [SwaggerOperation(OperationId = "UpdateClient")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> UpdateClient(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            [Required] string currentPassword, 
            string newPassword = null, 
            string userName = null, 
            string newClientName = null, 
            string newClientLastName = null, 
            string newClientAddress = null);

        [HttpDelete]
        [Route("~/{version}/Clients/")]
        [SwaggerOperation(OperationId = "DeleteClient")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> DeleteClient(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version,
            [Required] Guid clientId, 
            [Required] Guid sessionId);

    }
}

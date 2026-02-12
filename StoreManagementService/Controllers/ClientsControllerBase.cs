using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StoreManagementService.Controllers
{
    [ApiController]
    [ApiVersion("0.1")]
    [Route("[controller]")]
    public abstract class ClientsControllerBase : ControllerBase
    {
        [HttpPost]
        [Route("~/{version}/Clients/")]
        [SwaggerResponse(statusCode: 200, type: typeof(ActionResult), description: "Ok")]
        [SwaggerResponse(statusCode: 400, type: typeof(ActionResult), description: "Bab Request")]
        [SwaggerResponse(statusCode: 401, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> AddClient([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required, EmailAddress] string emailAddress, [Required] string password, [Required] string clientName, [Required] string clientLastName, [Required] string clientAddress);

        [HttpPost]
        [Route("~/{version}/Clients/")]
        [SwaggerResponse(statusCode: 200, type: typeof(ActionResult), description: "Ok")]
        [SwaggerResponse(statusCode: 400, type: typeof(ActionResult), description: "Bab Request")]
        [SwaggerResponse(statusCode: 401, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> Login([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required, EmailAddress] string emailAddress, [Required] string password);

        [HttpPut]
        [Route("~/{version}/Clients/")]
        [SwaggerResponse(statusCode: 200, type: typeof(ActionResult), description: "Ok")]
        [SwaggerResponse(statusCode: 400, type: typeof(ActionResult), description: "Bab Request")]
        [SwaggerResponse(statusCode: 401, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> UpdateClient([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] string currentPassword, string newPassword = null, string userName = null, string newClientName = null, string newClientLastName = null, string newClientAddress = null);

        [HttpDelete]
        [Route("~/{version}/Clients/")]
        [SwaggerResponse(statusCode: 200, type: typeof(ActionResult), description: "Ok")]
        [SwaggerResponse(statusCode: 400, type: typeof(ActionResult), description: "Bab Request")]
        [SwaggerResponse(statusCode: 401, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> DeleteClient([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId);

    }
}

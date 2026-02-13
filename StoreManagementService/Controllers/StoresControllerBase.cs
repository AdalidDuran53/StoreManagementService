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
    public abstract class StoresControllerBase : ControllerBase
    {
        [HttpPost]
        [Route("~/{version}/Stores/")]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CustomResponseCreatedExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status201Created, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> AddStore(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            [Required] string storeBranch, 
            [Required] string storeAddress);

        [HttpPost]
        [Route("~/{version}/Stores/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> GetStore(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            Guid? storeId = null);

        [HttpPut]
        [Route("~/{version}/Stores/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> UpdateStore(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required] Guid clientId, 
            [Required] Guid sessionId, 
            [Required] Guid storeId, 
            string newStoreBranch = null, 
            string newStoreAddress = null);

        [HttpDelete]
        [Route("~/{version}/Stores/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> DeleteStore(
            [FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")][DefaultValue("0.1")] string version, 
            [Required] Guid clientId,
            [Required] Guid sessionId, 
            Guid storeId);
    }
}

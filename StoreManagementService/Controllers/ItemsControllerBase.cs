using DTOs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
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
    public abstract class ItemsControllerBase : ControllerBase
    {
        [HttpPost]
        [Route("~/{version}/Items/")]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(CustomResponseCreatedExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status201Created, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> AddItem([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] string itemCode, [Required] string itemDescription, [Required] decimal itemPrice, [Required] IFormFile itemImg, [Required] int itemStock);

        [HttpPost]
        [Route("~/{version}/Items/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> GetItem([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid? itemId = null);

        [HttpPut]
        [Route("~/{version}/Items/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> UpdateItem([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId, [Required] Guid itemId,  string itemCode = null, string itemDescription = null, decimal? itemPrice = null, IFormFile itemImg = null, int? itemStock = null);

        [HttpDelete]
        [Route("~/{version}/Items/")]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CustomResponseOKExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status200OK, type: typeof(CustomResponse), description: "Ok")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CustomResponseBadRequestExample))]
        [SwaggerResponse(statusCode: StatusCodes.Status400BadRequest, type: typeof(ErrorResponse), description: "Bab Request")]
        [SwaggerResponse(statusCode: StatusCodes.Status401Unauthorized, type: typeof(ActionResult), description: "Unauthorized")]
        public abstract Task<IActionResult> DeleteItem([FromRoute][Required][RegularExpression("^(?<major>[0-9]+)\\.(?<minor>[0-9]+)$")] string version, [Required] Guid clientId, [Required] Guid sessionId, Guid itemId);
    }
}

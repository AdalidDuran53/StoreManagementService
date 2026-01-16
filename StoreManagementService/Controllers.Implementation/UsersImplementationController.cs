using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StoreManagementService.Controllers.Implementation
{
    [ApiVersion("0.1")]
    [ApiController]
    public class UsersImplementationController : UsersControllerBase
    {
        [HttpPost]
        [Route("~/{version::apiVersion}/Users/AddUser")]
        public override Task<IActionResult> AddUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password)
        {
            return Task.FromResult<IActionResult>(Ok("AddUser"));
        }

        [HttpDelete]
        [Route("~/{version::apiVersion}/Users/DeleteUser")]
        public override Task<IActionResult> DeleteUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] Guid userId, [Required] Guid sessionId)
        {
            return Task.FromResult<IActionResult>(Ok("AddUser"));
        }

        [HttpPost]
        [Route("~/{version::apiVersion}/Users/Login")]
        public override Task<IActionResult> Login([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] string userName, [Required] string password)
        {
            return Task.FromResult<IActionResult>(Ok("AddUser"));
        }
        [HttpPut]
        [Route("~/{version::apiVersion}/Users/UpdateUser")]
        public override Task<IActionResult> UpdateUser([FromRoute, RegularExpression("^(?<major>[0-9]+).(?<major>[0-9]+)$"), Required] string version, [Required] Guid userId, [Required] Guid sessionId, [Required] string currentPassword, string newPassword = null, string userName = null)
        {
            return Task.FromResult<IActionResult>(Ok("AddUser"));
        }
    }
}

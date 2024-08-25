using System.Security.Claims;
using API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    [HttpGet("unauthorized")]
    public IActionResult GetUnauthorized() => Unauthorized();

    [HttpGet("badrequest")]
    public IActionResult GetBadRequest() => BadRequest("Not a good request");

    [HttpGet("notfound")]
    public IActionResult GetNotFound() => NotFound();

    [HttpGet("internalerror")]
    public IActionResult GetInternalError() => throw new Exception("This is the test exception");

    [HttpPost("validationerror")]
    public IActionResult GetValidationError(CreateProductDto product) => Ok();

    [Authorize]
    [HttpGet("secret")]
    public IActionResult GetSecret()
    {
        var name = User.FindFirst(ClaimTypes.Name)?.Value;
        var id   = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Ok("Hello " + name + " with the Id of " + id);
    }
}

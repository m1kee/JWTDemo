using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JWTDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class PrivateController : ControllerBase
{
    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public ActionResult GetIfLogged()
    {
        return Ok("User is logged");
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize(Roles = "developer, qa")]
    public ActionResult GetIfHasAnyRoles()
    {
        return Ok("User has any of the required roles");
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize(Roles = "developer")]
    public ActionResult GetIfDeveloper()
    {
        return Ok("User is developer");
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize(Roles = "qa")]
    public ActionResult GetIfQA()
    {
        return Ok("User is QA");
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public ActionResult GetMyRoles()
    {
        var user = HttpContext.User;

        if (user == null || user.Identity == null)
            return BadRequest();

        var roles = user.Claims
            .Where(x => x.Type.Equals(ClaimTypes.Role))
            .Select(s => s.Value)
            .ToList();
        return Ok(roles);
    }
}

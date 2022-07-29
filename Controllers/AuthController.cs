using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JWTDemo.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JWTDemo.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{

    private readonly IConfiguration _config;
    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet]
    [Route("[action]")]
    public ActionResult GetToken(GetTokenRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserName))
                return BadRequest();

            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Sid, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, "Test User")
            };

            switch (request.UserName.ToLower())
            {
                case "admin":
                    claims.Add(new Claim(ClaimTypes.Role, "developer"));
                    claims.Add(new Claim(ClaimTypes.Role, "qa"));
                    break;
                case "developer":
                    claims.Add(new Claim(ClaimTypes.Role, "developer"));
                    break;
                case "qa":
                    claims.Add(new Claim(ClaimTypes.Role, "qa"));
                    break;
                default:
                    throw new UnauthorizedAccessException();
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("Jwt").GetValue<string>("Key")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _config.GetSection("Jwt").GetValue<string>("Issuer"),
                audience: _config.GetSection("Jwt").GetValue<string>("Audience"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(20),
                signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return Ok(jwt);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch(Exception)
        {
            throw;
        }
    }
}

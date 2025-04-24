using AuthenticationApi.Modules.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthenticationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JwtTokenController : ControllerBase
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenController(IOptions<JwtSettings> jwtOptions)
    {
        _ = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
        _jwtSettings = jwtOptions.Value ?? throw new ArgumentNullException(nameof(jwtOptions.Value));
    }

    [HttpGet]
    public IActionResult GetToken(string user)
    {
        if (!string.IsNullOrEmpty(user))
        {
            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(string username)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Currency.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Currency.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Возвращает токен для авторизации
    /// </summary>
    /// <param name="user">Данные пользователя для авторизации (для теста используется "test")</param>
    /// <response code="200"> Успешный возврат токена</response>
    /// <response code="401"> Ошибка авторизации</response>
    [HttpPost("token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult Token([FromBody] LoginModel user)
    {
        if (user.Username != "test" || user.Password != "test")
            return Unauthorized();
        
        var jwtSection = _configuration.GetSection("JwtConfig");
        var secret = jwtSection.GetValue<string>("Secret")!;
        var issuer = jwtSection.GetValue<string>("Issuer")!;
        var audience = jwtSection.GetValue<string>("Audience")!;
        var expiresIn = jwtSection.GetValue<string>("expiresIn");
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username)
        };
        
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(Convert.ToDouble(expiresIn)),
            signingCredentials: credentials
        );
        
        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookstore.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost("connect/token")]
        [Consumes("application/x-www-form-urlencoded")]
        public IActionResult GetClientToken([FromForm] string grant_type, [FromForm] string? client_id, [FromForm] string? client_secret)
        {
            if(Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var headerValue = authHeader.ToString();
                if(headerValue.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var base64Credentials = headerValue["Basic ".Length..].Trim();
                        var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(base64Credentials)).Split(':');
                        if(credentials.Length == 2)
                        {
                            client_id = credentials[0];
                            client_secret = credentials[1];
                        }
                    }
                    catch
                    {
                        return BadRequest(new { error = "invalid_basic_auth_header", error_description = "Invalid basic auth header" });
                    }
                }
            }

            if (grant_type != "client_credentials")
            {
                return BadRequest(new { error = "unsupported_grant_type" });
            }

            var validClientId = _configuration["OAuth:ClientCredentials:ClientId"];
            var validClientSecret = _configuration["OAuth:ClientCredentials:ClientSecret"];

            if (client_id != validClientId || client_secret != validClientSecret)
            {
                return Unauthorized(new { error = "invalid_client" });
            }

            var token = GenerateJwtToken("admin-client", scope: "books.crud");
            return Ok(new { access_token = token, token_type = "Bearer", expires_in = 3600 });
        }

        [HttpGet("authorize")]
        public IActionResult ImplicitAuthorize([FromQuery] string response_type, [FromQuery] string client_id, [FromQuery] string redirect_uri)
        {
            if (response_type != "token" || client_id != "swagger-ui")
            {
                return BadRequest(new { error = "unsupported_response_type_or_client" });
            }

            var validRedirectUri = _configuration["OAuth:Implicit:RedirectUri"];

            if (redirect_uri != validRedirectUri)
            {
                return BadRequest(new {error = "invalid_redirect_uri"});
            }

            var token = GenerateJwtToken("swagger-user", scope: "books.search");

            return Redirect($"{redirect_uri}#access_token={Uri.EscapeDataString(token)}&token_type=Bearer&expires_in=3600");
        }

        private string GenerateJwtToken(string identity, string scope)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, identity),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new ("scope", scope)
            };

            var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpiryInMinutes"]!)),
            signingCredentials: creds
        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

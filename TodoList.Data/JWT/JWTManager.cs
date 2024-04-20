using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoList.Data.Domain;

namespace TodoList.Data.JWT;

public static class JWTManager
{
    private const string SECURITY_KEY = "q2423424ergwerrq23<capybaras_slay>144131233412123";
    private const string ISSUER = "Capybara";
    private const string AUDIENCE = "Capybaras Community";
    public const double TOKEN_EXPIRATION_HOURS = 1.0;
    public const string TOKEN_COOKIES_KEY = "jwt";

    public static string GenerateJwtToken(TodoListUser user)
    {
        if (user == null || user.Email == null)
        {
            throw new InvalidOperationException("User must not be null.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new (JwtRegisteredClaimNames.Email, user.Email),
        };

        var token = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(TOKEN_EXPIRATION_HOURS), 
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static TokenValidationParameters GetTokenValidationParameters()
        => new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = ISSUER,
            ValidAudience = AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SECURITY_KEY))
        };
}

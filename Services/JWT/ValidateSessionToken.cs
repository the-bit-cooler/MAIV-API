using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class JwtService
{
  internal static string ValidateSessionToken(string token)
  {
    JwtSecurityTokenHandler tokenHandler = new();

    ClaimsPrincipal principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = GetSecurityKey(),
      ValidateIssuer = true,
      ValidIssuer = "maiv",
      ValidateAudience = true,
      ValidAudience = "maiv",
      ValidateLifetime = true,
      ClockSkew = TimeSpan.Zero
    }, out SecurityToken validatedToken);

    string? email = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

    if (string.IsNullOrEmpty(email)) throw new Exception("Invalid or expired session.");

    return email;
  }
}

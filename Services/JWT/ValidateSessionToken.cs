using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class JwtService
{
  internal static void ValidateSessionToken(string token)
  {
    new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
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
  }
}

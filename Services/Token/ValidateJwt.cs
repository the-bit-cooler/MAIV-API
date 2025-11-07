using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class TokenService
{
  public JwtSecurityToken? ValidateJwt(string token)
  {
    try
    {
      JwtSecurityTokenHandler jwtHandler = new();

      var parameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
      };

      jwtHandler.ValidateToken(token, parameters, out var validatedToken);

      return (JwtSecurityToken)validatedToken;
    }
    catch
    {
      return null;
    }
  }
}

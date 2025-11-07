using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class TokenService
{
  public ClaimsPrincipal? ValidateJwt(string token)
  {
    var handler = new JwtSecurityTokenHandler();
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

    try
    {
      var parameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = key,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
      };

      return handler.ValidateToken(token, parameters, out _);
    }
    catch
    {
      return null;
    }
  }
}

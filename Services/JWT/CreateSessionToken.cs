using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class JwtService
{
  internal static string CreateSessionToken(string email)
  {
    SigningCredentials credentials = new(GetSecurityKey(), SecurityAlgorithms.HmacSha256);

    Claim[] claims =
    [
      new Claim(ClaimTypes.Email, email)
    ];

    JwtSecurityToken jwtToken = new JwtSecurityToken(
      issuer: "maiv",
      audience: "maiv",
      claims: claims,
      expires: DateTime.UtcNow.AddDays(30),
      signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(jwtToken);
  }
}

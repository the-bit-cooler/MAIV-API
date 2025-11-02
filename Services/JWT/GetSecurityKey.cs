using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ScripturAI.Services;

public partial class JwtService
{
  internal static SymmetricSecurityKey GetSecurityKey()
  {
    if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET"))) throw new Exception("Missing jwt secret!");

    return new(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")!));
  }
}

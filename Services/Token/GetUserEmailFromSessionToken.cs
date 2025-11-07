using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace ScripturAI.Services;

public partial class TokenService
{
  public string? GetUserIdFromSessionToken(HttpHeadersCollection headers)
  {
    var securityToken = ValidateSessionToken(headers);
    return securityToken?.Subject ?? securityToken?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
  }
}

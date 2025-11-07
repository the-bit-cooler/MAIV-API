using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;

namespace ScripturAI.Services;

public partial class TokenService
{
  public string? GetUserEmailFromSessionToken(HttpHeadersCollection headers)
  {
    return ValidateSessionToken(headers)?.FindFirst(ClaimTypes.Email)?.Value;
  }
}

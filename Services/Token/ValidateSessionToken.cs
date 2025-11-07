using System.Security.Claims;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace ScripturAI.Services;

public partial class TokenService
{
  public ClaimsPrincipal? ValidateSessionToken(HttpHeadersCollection headers, bool throwOnFailure = false)
  {
    try
    {
      if (!headers.TryGetValues("Authorization", out var authHeaders))
      {
        if (throwOnFailure)
          throw new UnauthorizedAccessException("Missing Authorization header.");
        return null;
      }

      var authHeader = authHeaders.FirstOrDefault();
      if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      {
        if (throwOnFailure)
          throw new UnauthorizedAccessException("Malformed Authorization header.");
        return null;
      }

      var token = authHeader["Bearer ".Length..].Trim();
      if (string.IsNullOrEmpty(token))
      {
        if (throwOnFailure)
          throw new UnauthorizedAccessException("Empty token.");
        return null;
      }

      var principal = ValidateJwt(token);

      if (principal == null)
      {
        if (throwOnFailure)
          throw new UnauthorizedAccessException("Invalid or expired token.");
        return null;
      }

      return principal;
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Error validating session token.", nameof(ValidateSessionToken));
      if (throwOnFailure)
        throw;
      return null;
    }
  }
}

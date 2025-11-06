using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ScripturAI.Services;

namespace ScripturAI;

public class ValidateLoginSession(TokenService tokenService, ILogger<ValidateLoginSession> logger)
{
  internal record RequestBody(string sessionToken);
  private readonly TokenService tokenService = tokenService;
  private readonly ILogger<ValidateLoginSession> logger = logger;

  [Function("ValidateLoginSession")]
  public IActionResult Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "validate-login-session")] HttpRequestData req)
  {
    try
    {
      // 1️⃣ Extract Bearer token
      if (!req.Headers.TryGetValues("Authorization", out var authHeaderValues))
      {
        return new BadRequestResult();
      }

      var authHeader = authHeaderValues.FirstOrDefault();
      if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
      {
        return new BadRequestResult();
      }

      string token = authHeader["Bearer ".Length..].Trim();
      if (string.IsNullOrEmpty(token))
      {
        return new BadRequestResult();
      }

      // 2️⃣ Validate
      var principal = tokenService.ValidateJwt(token);

      if (principal == null)
      {
        return new UnauthorizedResult();
      }

      return new OkResult();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(ValidateLoginSession));

      return new StatusCodeResult(500);
    }
  }
}

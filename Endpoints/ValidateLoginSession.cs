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
      tokenService.ValidateSessionToken(req.Headers, throwOnFailure: true);

      return new OkResult();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(ValidateLoginSession));

      return new UnauthorizedResult();
    }
  }
}

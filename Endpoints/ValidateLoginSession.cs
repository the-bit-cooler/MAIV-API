using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ScripturAI.Services;

namespace ScripturAI;

public class ValidateLoginSession(ILogger<ValidateLoginSession> logger)
{
  internal record RequestBody(string sessionToken);
  private readonly ILogger<ValidateLoginSession> logger = logger;

  [Function("ValidateLoginSession")]
  public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "validate-login-session")] HttpRequestData req)
  {
    try
    {
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      RequestBody? data = JsonSerializer.Deserialize<RequestBody>(requestBody);

      string? sessionToken = data?.sessionToken;
      if (string.IsNullOrEmpty(sessionToken))
      {
        return new BadRequestResult();
      }

      JwtService.ValidateSessionToken(sessionToken);

      return new OkResult();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(VerifyMagicLink));

      return new BadRequestResult();
    }
  }
}

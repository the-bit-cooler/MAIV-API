using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;

namespace ScripturAI;

public class VerifyMagicLink(DataService dataService, ILogger<VerifyMagicLink> logger)
{
  internal record RequestBody(string email, string token);
  internal record User(string id, string collection, string token, string tokenExpiry, bool tokenUsed);
  private readonly DataService dataService = dataService;
  private readonly ILogger<VerifyMagicLink> logger = logger;

  [Function("VerifyMagicLink")]
  public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "verify-magic-link")] HttpRequestData req)
  {
    try
    {
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      RequestBody? data = JsonSerializer.Deserialize<RequestBody>(requestBody);

      string? email = data?.email;
      string? token = data?.token;
      if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
      {
        return new ContentResult
        {
          Content = "Magic Link is broken and likely missing link parameters.",
          ContentType = "text/plain",
          StatusCode = 400
        };
      }

      var user = await dataService.GetUserAsync(email, token);

      return new ContentResult
      {
        Content = JwtService.CreateSessionToken(user.id),
        ContentType = "text/plain",
        StatusCode = 200
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(VerifyMagicLink));

      return new ContentResult
      {
        Content = "Sorry, we are having trouble logging you in. Please try again later.",
        ContentType = "text/plain",
        StatusCode = 500
      };
    }
  }
}

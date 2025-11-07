using System.Net;
using System.Text.Json;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using ScripturAI.Services;

namespace ScripturAI.Functions;

public class AuthSignIn(DataService dataService, TokenService tokenService, ILogger<AuthSignIn> logger)
{
  const int FreeTierLimitFallback = 100;
  internal record RequestBody(string idToken);
  private readonly DataService dataService = dataService;
  private readonly TokenService tokenService = tokenService;
  private readonly ILogger<AuthSignIn> logger = logger;

  [Function("AuthSignIn")]
  public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth-sign-in/{provider}")] HttpRequestData req, string provider)
  {
    try
    {
      string? userId = null;

      try
      {
        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        RequestBody? data = JsonSerializer.Deserialize<RequestBody>(requestBody);

        var idToken = data?.idToken;
        if (string.IsNullOrEmpty(idToken))
        {
          throw new Exception("Identity token missing.");
        }

#if DEBUG
        // 🧪 Short-circuit local test mode
        if (provider == "test")
        {
          userId = "1234567890";
          logger.LogWarning("⚠️ Short-circuit auth in DEBUG mode - skipping external validation");
        }
        else
#endif
        if (provider == "google")
        {
          var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
          userId = payload.Subject;
        }
        else if (provider == "apple")
        {
          var jwt = await tokenService.ValidateAppleIdTokenAsync(idToken);
          userId = jwt.Subject ?? jwt.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        }

        if (string.IsNullOrEmpty(userId))
        {
          throw new Exception("Subject missing from identity token.");
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "{Caller}(): Token validation failed", nameof(AuthSignIn));
        return new UnauthorizedResult();
      }

      // Lookup or create user
      try
      {
        await dataService.GetDataContainer().ReadItemAsync<Models.User>(userId, new PartitionKey(nameof(User)));
      }
      catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
      {
        // somewhere during user creation or initialization
        var freeTierEnv = Environment.GetEnvironmentVariable("FREE_TIER_LIMIT");

        int freeTier = FreeTierLimitFallback;
        if (!string.IsNullOrEmpty(freeTierEnv) && int.TryParse(freeTierEnv, out var parsed))
        {
          freeTier = parsed;
        }

        await dataService.GetDataContainer().CreateItemAsync(
          new Models.User
          {
            id = userId,
            FreeTier = freeTier,
            CreatedAt = DateTime.UtcNow
          },
          new PartitionKey(nameof(User))
        );
      }

      return new JsonResult(new
      {
        sessionToken = tokenService.GenerateJwt(userId)
      });
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(AuthSignIn));

      return new StatusCodeResult(500)
;
    }
  }
}

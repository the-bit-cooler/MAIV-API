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
  internal record RequestBody(string idToken);
  private readonly DataService dataService = dataService;
  private readonly TokenService tokenService = tokenService;
  private readonly ILogger<AuthSignIn> logger = logger;

  [Function("AuthSignIn")]
  public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth-signin/{provider}")] HttpRequestData req, string provider)
  {
    try
    {
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      RequestBody? data = JsonSerializer.Deserialize<RequestBody>(requestBody);

      var idToken = data?.idToken;
      if (string.IsNullOrEmpty(idToken))
      {
        return new BadRequestResult();
      }

      string? email = null;
      string? name = null;

      try
      {
#if DEBUG
        // 🧪 Short-circuit local test mode
        if (provider == "test")
        {
          email = "tester@example.com";
          name = "Local Test";
          logger.LogWarning("⚠️ Short-circuit auth in DEBUG mode - skipping external validation");
        }
        else
#endif
        if (provider == "google")
        {
          var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
          email = payload.Email;
          name = payload.Name;
        }
        else if (provider == "apple")
        {
          var jwt = await tokenService.ValidateAppleIdTokenAsync(idToken);
          email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
          name = jwt.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
        }
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "{Caller}(): Token validation failed", nameof(AuthSignIn));
        return new UnauthorizedResult();
      }

      if (string.IsNullOrEmpty(email))
      {
        logger.LogError("{Caller}(): No email claim in token", nameof(AuthSignIn));
        return new NotFoundResult();
      }

      // Lookup or create user
      try
      {
        await dataService.GetDataContainer().ReadItemAsync<Models.User>(email, new PartitionKey(nameof(User)));
      }
      catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
      {
        var user = new Models.User
        {
          id = email,
          Name = name,
          Provider = provider,
          CreatedAt = DateTime.UtcNow
        };

        await dataService.GetDataContainer().UpsertItemAsync(
          user,
          new PartitionKey(nameof(User))
        );
      }

      return new JsonResult(new
      {
        sessionToken = tokenService.GenerateJwt(email),
        email
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

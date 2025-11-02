using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;
using System.Text.Json;

namespace ScripturAI;

public class Login(DataService dataService, EmailService emailService, ILogger<Login> logger)
{
  internal record RequestBody(string email);
  private readonly DataService dataService = dataService;
  private readonly EmailService emailService = emailService;
  private readonly ILogger<Login> logger = logger;

  [Function("Login")]
  public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "login")] HttpRequestData req)
  {
    try
    {
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      RequestBody? data = JsonSerializer.Deserialize<RequestBody>(requestBody);

      string? email = data?.email;
      if (!EmailService.ValidateEmail(email))
      {
        return new BadRequestResult();
      }

      var user = await dataService.GetUserAsync(email!);

      await emailService.SendMagicLinkAsync(user.id, user.token);

      logger.LogInformation("{Caller}(): Magic link sent to {email}", nameof(Login), user.id);

      return new OkResult();
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(Login));

      return new StatusCodeResult(500)
;
    }
  }
}

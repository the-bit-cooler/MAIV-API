using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;

namespace ScripturAI;

public class Login(DataService dataService, EmailService emailService, ILogger<Login> logger)
{
  private readonly DataService dataService = dataService;
  private readonly EmailService emailService = emailService;
  private readonly ILogger<Login> logger = logger;

  [Function("Login")]
  public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "get", Route = "login/{email}")] HttpRequestData req,
    string email)
  {
    try
    {
      if (!EmailService.ValidateEmail(email))
      {
        return new ContentResult
        {
          Content = "Please use a valid email",
          ContentType = "text/plain",
          StatusCode = 400
        };
      }

      var user = await dataService.GetUserAsync(email);

      await emailService.SendMagicLinkAsync(user.id, user.token);

      logger.LogInformation("{Caller}(): Magic link sent to {email}", nameof(Login), user.id);

      return new ContentResult
      {
        Content = $"Please check your inbox (or spam) folder for an email from us at {Environment.GetEnvironmentVariable("AWS_SES_SENDER_EMAIL")}.\n\n. Then click on the link to login!",
        ContentType = "text/plain",
        StatusCode = 200
      };
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{Caller}(): Server Error", nameof(Login));

      return new ContentResult
      {
        Content = "Sorry, we are having trouble logging you in. Please try again later.",
        ContentType = "text/plain",
        StatusCode = 500
      };
    }
  }
}

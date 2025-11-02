using Amazon;
using Amazon.SimpleEmail;
using Microsoft.Extensions.Logging;

namespace ScripturAI.Services;

public partial class EmailService
{
  private readonly AmazonSimpleEmailServiceClient emailClient = new(Environment.GetEnvironmentVariable("AWS_SES_ACCESS_KEY_ID"), Environment.GetEnvironmentVariable("AWS_SES_SECRET_ACCESS_KEY"), RegionEndpoint.GetBySystemName(Environment.GetEnvironmentVariable("AWS_SES_REGION")));
  private readonly ILogger<EmailService> logger;

  public EmailService(ILogger<EmailService> logger)
  {
    this.logger = logger;
  }
}

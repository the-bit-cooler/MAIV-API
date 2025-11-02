using Amazon.SimpleEmail.Model;

namespace ScripturAI.Services;

public partial class EmailService
{
  internal async Task SendMagicLinkAsync(string email, string token)
  {
    // Build magic link
    string magicLink = $"maiv://verify-magic-link?email={Uri.EscapeDataString(email)}&token={token}";

    // Prepare email request
    var sendRequest = new SendEmailRequest
    {
      Source = Environment.GetEnvironmentVariable("AWS_SES_SENDER_EMAIL"), // Verified in SES
      // Source = "maiv.scripturai.ai", // Verified in SES
      Destination = new Destination { ToAddresses = [email] },
      Message = new Message
      {
        Subject = new Content("Magic Link Login"),
        Body = new Body
        {
          Html = new Content($"<p>Click <a href=\"{magicLink}\">here</a> to log in.</p><p>If you do not see a link above, copy the following and try pasting it in your browser:</p><br />{magicLink}"),
        }
      }
    };

    SendEmailResponse response = await emailClient.SendEmailAsync(sendRequest);
  }
}

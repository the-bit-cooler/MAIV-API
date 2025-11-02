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
      Destination = new Destination { ToAddresses = [email] },
      Message = new Message
      {
        Subject = new Content("Your MAIV Magic Link Login"),
        Body = new Body
        {
          Html = new Content($@"
            <html>
              <body style=""font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Helvetica, Arial, sans-serif; color: #333; background-color: #f9f9f9; padding: 24px;"">
                <div style=""max-width: 600px; margin: 0 auto; background: #fff; border-radius: 8px; padding: 24px; box-shadow: 0 1px 3px rgba(0,0,0,0.1);"">
                  <h2 style=""text-align: center; color: #222;"">Magic Link Login</h2>
                  <p>Hi there,</p>
                  <p>Click the button below to securely log in to your <strong>MAIV</strong> account:</p>
                  <div style=""text-align: center; margin: 32px 0;"">
                    <a href=""{magicLink}"" style=""display: inline-block; padding: 12px 24px; background-color: #3b82f6; color: white; text-decoration: none; border-radius: 6px; font-weight: 600;"">Log in to MAIV</a>
                  </div>
                  <p>If the button above doesn't work, copy and paste the following link into your browser:</p>
                  <p style=""word-break: break-all; color: #555;"">{magicLink}</p>
                  <hr style=""margin: 32px 0; border: none; border-top: 1px solid #eee;"" />
                  <p style=""font-size: 12px; color: #888; text-align: center;"">
                    This link will expire soon for your security.<br/>
                    If you didn’t request this email, you can safely ignore it.
                  </p>
                </div>
              </body>
            </html>"
          )
        }
      }
    };

    SendEmailResponse response = await emailClient.SendEmailAsync(sendRequest);
  }
}

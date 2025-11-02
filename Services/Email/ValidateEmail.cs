using System.Net.Mail;

namespace ScripturAI.Services;

public partial class EmailService
{
  internal static bool ValidateEmail(string? email)
  {
    if (string.IsNullOrWhiteSpace(email)) return false;

    try
    {
      MailAddress addr = new(email);
      return addr.Address == email;
    }
    catch
    {
      return false;
    }
  }
}

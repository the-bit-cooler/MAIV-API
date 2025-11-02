using System.Net.Mail;

namespace ScripturAI.Services;

public partial class EmailService
{
  internal static bool ValidateEmail(string email)
  {
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

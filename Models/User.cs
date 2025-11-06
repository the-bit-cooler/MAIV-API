namespace ScripturAI.Models;

public class User
{
  public string? id { get; set; } // email
  public string collection => nameof(User); // partition key
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  public List<Passkey>? Passkeys { get; set; } = [];
}

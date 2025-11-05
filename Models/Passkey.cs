namespace ScripturAI.Models;

public class Passkey
{
  public string Id { get; set; } = string.Empty;
  public string PublicKey { get; set; } = string.Empty;
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

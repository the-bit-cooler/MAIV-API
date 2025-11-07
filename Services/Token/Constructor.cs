using Microsoft.Extensions.Logging;

namespace ScripturAI.Services;

public partial class TokenService(ILogger<TokenService> logger, HttpClient httpClient)
{
  private readonly string secret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
  private readonly string issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
  private readonly string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
  private readonly HttpClient httpClient = httpClient;
  private readonly ILogger<TokenService> logger = logger;
}

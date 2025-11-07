using Microsoft.Extensions.Logging;

namespace ScripturAI.Services;

public partial class TokenService(DataService dataService, HttpClient httpClient, ILogger<TokenService> logger)
{
  private readonly string secret = Environment.GetEnvironmentVariable("JWT_SECRET")!;
  private readonly string issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
  private readonly string audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")!;
  private readonly DataService dataService = dataService;
  private readonly HttpClient httpClient = httpClient;
  private readonly ILogger<TokenService> logger = logger;
}

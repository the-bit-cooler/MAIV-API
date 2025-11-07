using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;

namespace ScripturAI;

public class TranslateBibleVerseToModernEnglish(AiService aiService, TokenService tokenService)
{
  private readonly AiService aiService = aiService;
  private readonly TokenService tokenService = tokenService;

  [Function("TranslateBibleVerseToModernEnglish")]
  public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "bible/{version}/{book}/{chapter}/{verse}/translate/{mode}")] HttpRequestData req,
      string version,
      string book,
      int chapter,
      int verse,
      string mode
    )
  {
    if (!Enum.TryParse(mode, true, out AiService.Mode aiMode))
      aiMode = AiService.Mode.Devotional; // default fallback

    return new ContentResult
    {
      Content = await aiService.TranslateBibleVerseToModernEnglishAsync(
        userId: tokenService.GetUserIdFromSessionToken(req.Headers),
        aiMode,
        version,
        book,
        chapter,
        verse,
        caller: $"{book}:{chapter}:{verse}:{version}"
      ),
      ContentType = "text/plain",
      StatusCode = 200
    };
  }
}

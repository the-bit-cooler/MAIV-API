using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;

namespace ScripturAI;

public class ExplainBibleVerse
{
  private readonly AiService aiService;

  public ExplainBibleVerse(AiService aiService)
  {
    this.aiService = aiService;
  }

  [Function("ExplainBibleVerse")]
  public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "bible/{version}/{book}/{chapter}/{verse}/explain/{mode}")] HttpRequestData req,
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
      Content = await aiService.ExplainBibleVerseAsync(
        aiMode,
        version,
        book,
        chapter,
        verse,
        caller: $"{nameof(ExplainBibleVerse)}({aiMode})"
      ),
      ContentType = "text/plain",
      StatusCode = 200
    };
  }
}

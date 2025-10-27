using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.AspNetCore.Mvc;
using ScripturAI.Services;

namespace ScripturAI;

public class IllustrateBibleVerse
{
  private readonly AiService aiService;

  public IllustrateBibleVerse(AiService aiService)
  {
    this.aiService = aiService;
  }

  [Function("IllustrateBibleVerse")]
  public async Task<IActionResult> Run(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "bible/{version}/{book}/{chapter}/{verse}/illustrate")] HttpRequestData req,
      string version,
      string book,
      int chapter,
      int verse
    )
  {
    return new ContentResult
    {
      Content = await aiService.IllustrateBibleVerseAsync(
        version,
        book,
        chapter,
        verse,
        caller: $"{nameof(IllustrateBibleVerse)}()"
      ),
      ContentType = "text/plain",
      StatusCode = 200
    };
  }
}

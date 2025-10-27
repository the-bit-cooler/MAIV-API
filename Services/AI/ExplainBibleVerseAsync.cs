using System.ClientModel;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace ScripturAI.Services;

public partial class AiService
{
  /// <summary>
  /// Returns either a url to where the explanation was stored or an empty string if none can be obtained.
  /// </summary>
  internal async Task<string> ExplainBibleVerseAsync(
    Mode mode,
    string version,
    string book,
    int chapter,
    int verse,
    string caller
  )
  {
    string callerId = $"{caller}->{nameof(AiService)}.{nameof(GetChatCompletionAsync)}";

    try
    {
      BlobClient blobClient = await dataService.GetBlobClientAsync($"explanation/{version}/{book}/{chapter}/{verse}/{mode}.txt");

      if (await blobClient.ExistsAsync()) return blobClient.Uri.ToString();

      List<ChatMessage> messages =
      [
        mode.GetSystemBehavior(),
      ];

      var bibleChapter = await dataService.GetBibleChapterAsync(version, book, chapter, callerId);
      if ((bibleChapter?.Count ?? 0) > 0)
      {
        messages.Add(new SystemChatMessage($@"
          Full Bible Chapter from {version}: 
          {string.Join("\n", bibleChapter!.Select(v => $"{v.book} {v.chapter}: {v.text}"))}
        "));
      }

      messages.Add(new UserChatMessage($@"
        Explain {book}:{chapter}:{verse} from the {version} version of the Bible. 
        Do not use a title with the verse reference or quote at the top of your GitHub markdown response. 
        Just go right into your explanation.
        Mode: {mode.GetDisplayName()}. 
        Focus level: {(mode == Mode.Pastoral ? "scholarly" : mode == Mode.Study ? "educational" : "inspirational")}.
      "));

      int attempt = 1;
      const int MAX_ATTEMPTS = 3;

      while (true)
      {
        ClientResult<ChatCompletion> response = await GetChatClient().CompleteChatAsync(messages);

        var chatCompletion = response.Value;

        if (chatCompletion == null)
        {
          if (attempt >= MAX_ATTEMPTS)
            throw new Exception($"No response received. Retried model {attempt} times.");
        }
        else if (chatCompletion.Content == null || chatCompletion.Content.Count == 0)
        {
          if (attempt >= MAX_ATTEMPTS)
            throw new Exception($"Finish reason: {chatCompletion.FinishReason}. Retried model {attempt} times.");
        }
        else
        {
          string chat = string.Join("\n", chatCompletion.Content
            .Where(c => !string.IsNullOrWhiteSpace(c.Text))
            .Select(c => c.Text));

          if (!string.IsNullOrWhiteSpace(chat))
          {

            // 5️⃣ Upload to Azure Storage
            using MemoryStream stream = new(Encoding.UTF8.GetBytes(chat));

            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
          }

          if (attempt >= MAX_ATTEMPTS)
            throw new Exception($"Empty response received. Retried model {attempt} times.");
        }

        await Task.Delay(1000 * attempt);
        attempt++;
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{CallerId}: An error occurred while fetching an explanation.", callerId);
    }

    return string.Empty;
  }
}

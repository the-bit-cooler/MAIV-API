using System.ClientModel;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;

namespace ScripturAI.Services;

public partial class AiService
{
  internal async Task<string> TranslateBibleVerseToModernEnglishAsync(
    Mode mode,
    string version,
    string book,
    int chapter,
    int verse,
    string caller
  )
  {
    string callerId = $"{caller}->{nameof(AiService)}.{nameof(TranslateBibleVerseToModernEnglishAsync)}";

    try
    {
      BlobClient blobClient = await dataService.GetBlobClientAsync($"translation/{version}/{book}/{chapter}/{verse}/{mode}.txt");

      if (await blobClient.ExistsAsync()) return blobClient.Uri.ToString();

      string documentId = $"{book}:{chapter}:{verse}:{version}";

      var selectedVerse = await dataService.GetBibleVerseAsync(documentId, book, callerId);
      if (selectedVerse == null)
      {
        throw new Exception("Failed to get verse from database.");
      }

      List<ChatMessage> messages =
      [
         new SystemChatMessage(@"
          You are a Bible-believing translation assistant that always responds in GitHub-style Markdown. 
          When given a verse from an older bible version, translate it into clear, natural modern English that accurately reflects the meaning of the original Hebrew, Aramaic, or Greek text. 
          You may rephrase expressions to match their sense in the original languages while keeping the tone readable and faithful. 
          Write in your own words with a style similar to modern translations like the NIV or NKJV, but do not copy from them. 
          Return translated verse at the top of your response (no need to re-quote the original) and follow it with the reasoning behind your translation.
        "),
        new UserChatMessage($@"
          {book} {chapter}:{verse} from the {version}: {selectedVerse.text}. 
          Mode: {mode.GetDisplayName()}. 
          Focus level: {(mode == Mode.Pastoral ? "scholarly" : mode == Mode.Study ? "educational" : "inspirational")}.
        "),
        new UserChatMessage($"{book} {chapter}:{verse} from the {version}: {selectedVerse.text}.")
      ];

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
      logger.LogError(ex, "{CallerId}: An error occurred while fetching a translation.", callerId);
    }

    return string.Empty;
  }
}

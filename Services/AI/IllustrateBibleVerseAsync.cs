using System.ClientModel;
using System.Text;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using OpenAI.Chat;
using OpenAI.Images;

namespace ScripturAI.Services;

public partial class AiService
{
  /// <summary>
  /// Returns either a url to where the new illustration was stored or an empty string if none can be obtained.
  /// </summary>
  internal async Task<string> IllustrateBibleVerseAsync(
    string version,
    string book,
    int chapter,
    int verse,
    string caller
  )
  {
    string callerId = $"{caller}->{nameof(AiService)}.{nameof(IllustrateBibleVerseAsync)}";

    try
    {
      BlobClient blobClient = await dataService.GetBlobClientAsync($"illustration/{version}/{book}/{chapter}/{verse}.png");

      if (await blobClient.ExistsAsync()) return blobClient.Uri.ToString();

      string documentId = $"{book}:{chapter}:{verse}:{version}";

      var selectedVerse = await dataService.GetBibleVerseAsync(documentId, book, callerId);
      if (selectedVerse == null)
      {
        throw new Exception("Failed to get verse from database.");
      }

      string imagePrompt = $@"
        You are a visual designer specializing in sacred art. 
        Create a symbolic, reverent, and family-safe classical-style illustration of the Bible verse {book} {chapter}:{verse}.

        Verse text:
        ""{selectedVerse.text}""

        Guidelines:
        - Focus on the verse's central meaning or emotion.
        - Depict concepts symbolically (faith, light, redemption, peace).
        - Style: renaissance-inspired or traditional sacred art.
        - Avoid modern elements, technology, or text overlays (except for the verse itself).
        - Avoid imagery of blood, weapons, violence, nudity, or suffering.
        - The image should feel peaceful, timeless, and reverent.
      ";

      int attempt = 1;
      const int MAX_ATTEMPTS = 3;

      while (true)
      {
        try
        {
#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
          GeneratedImage image = await GetImageClient().GenerateImageAsync(
            imagePrompt,
            new ImageGenerationOptions
            {
              Size = GeneratedImageSize.W1024xH1536
            }
          );
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

          // 5️⃣ Upload to Azure Storage
          using var stream = new MemoryStream();

          image.ImageBytes.ToStream().CopyTo(stream);
          stream.Position = 0;

          await blobClient.UploadAsync(stream, overwrite: true);

          return blobClient.Uri.ToString();
        }
        catch (Exception ex)
        {
          if (attempt >= MAX_ATTEMPTS)
          {
            logger.LogError(ex.Message);
            break;
          }
        }

        await Task.Delay(1000 * attempt);
        attempt++;
      }
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{CallerId}: An error occurred while fetching an illustration.", callerId);
    }

    return string.Empty;
  }
}

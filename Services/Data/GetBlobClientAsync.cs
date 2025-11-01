using Azure.Storage.Blobs;

namespace ScripturAI.Services;

public partial class DataService
{
  internal async Task<BlobClient> GetBlobClientAsync(string blobName)
  {
    return (await GetBlobContainerAsync()).GetBlobClient(blobName.Replace(" ", string.Empty));
  }
}

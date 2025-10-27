using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ScripturAI.Services;

public partial class DataService
{
  internal async Task<BlobClient> GetBlobClientAsync(string blobName)
  {
    return (await GetBlobContainerAsync()).GetBlobClient(blobName.Replace(" ", string.Empty));
  }
}

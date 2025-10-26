using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal Container GetCacheContainer()
  {
    return GetDatabase()
      .GetContainer(Environment.GetEnvironmentVariable("COSMOS_CACHE_CONTAINER_NAME"));
  }
}

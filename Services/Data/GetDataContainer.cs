using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal Container GetDataContainer()
  {
    return GetDatabase()
      .GetContainer(Environment.GetEnvironmentVariable("COSMOS_DATA_CONTAINER_NAME"));
  }
}

using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal Container GetUsersContainer()
  {
    return GetDatabase()
      .GetContainer(Environment.GetEnvironmentVariable("COSMOS_USERS_CONTAINER_NAME"));
  }
}

using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal Database GetDatabase()
  {
    return dbClient
      .GetDatabase(Environment.GetEnvironmentVariable("COSMOS_DATABASE_NAME"));
  }
}

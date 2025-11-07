using System.Net;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace ScripturAI.Services;

public partial class DataService
{
  internal async Task<string> ConsumeUserCreditAsync(string? email, string caller)
  {
    string callerId = $"{caller}->{nameof(AiService)}.{nameof(ConsumeUserCreditAsync)}";

    // Check whether user has any API credits
    if (string.IsNullOrEmpty(email)) return "signup";

    try
    {
      var container = GetDataContainer();
      var response = await container.ReadItemAsync<Models.User>(email, new PartitionKey(nameof(User)));
      var user = response.Resource;

      if (user.PaidTier > 0)
      {
        // decrement paid tier
        await container.PatchItemAsync<Models.User>(
          id: user.id,
          partitionKey: new PartitionKey(nameof(User)),
          patchOperations:
          [
            PatchOperation.Increment("/PaidTier", -1)
          ]);

        return "ok";
      }
      else if (user.FreeTier > 0)
      {
        // decrement free tier
        await container.PatchItemAsync<Models.User>(
          id: user.id,
          partitionKey: new PartitionKey(nameof(User)),
          patchOperations:
          [
            PatchOperation.Increment("/FreeTier", -1)
          ]);

        return "ok";
      }
      else
      {
        // no credits remaining
        return "exhausted";
      }
    }
    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
    {
      logger.LogError(ex, "{CallerId}: Failed to retrieve user.", callerId);
      return "retry";
    }
    catch (Exception ex)
    {
      logger.LogError(ex, "{CallerId}: Unexpected error during user credit check.", callerId);
      return "error";
    }
  }
}

using Microsoft.Azure.Cosmos;

namespace ScripturAI.Services;

public partial class DataService
{
  internal record User(string id, string collection, string token, string tokenExpiry);
  internal async Task<User> GetUserAsync(string email, string? token = null)
  {
    Container usersContainer = GetUsersContainer();
    ItemResponse<User> userResponse;

    if (string.IsNullOrEmpty(token))
    {
      // then create new login token
      token = Guid.NewGuid().ToString();
      string expiry = DateTime.UtcNow.AddMinutes(15).ToString("o"); // ISO format

      // Create or update the user
      userResponse = await usersContainer.UpsertItemAsync<User>(
        new(email, nameof(User), token, expiry),
        new PartitionKey(nameof(User))
      );
    }
    else
    {
      // User exist so compare token
      userResponse = await usersContainer.ReadItemAsync<User>(email, new PartitionKey(nameof(User)));

      User user = userResponse.Resource;

      if (token != user.token || DateTime.Parse(user.tokenExpiry) < DateTime.UtcNow)
      {
        throw new Exception("Invalid or expired magic link.");
      }
    }

    return userResponse.Resource;
  }
}

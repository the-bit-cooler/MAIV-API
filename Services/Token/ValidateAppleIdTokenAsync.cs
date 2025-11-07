using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace ScripturAI.Services;

public partial class TokenService
{
  const string ApplePublicKeys = "ApplePublicKeys";
  const int TwentyFourHours = 86400;
  private record ApplePublicKeysCache(string id = ApplePublicKeys, string collection = ApplePublicKeys, string jwks = "", int ttl = TwentyFourHours);
  internal async Task<JwtSecurityToken> ValidateAppleIdTokenAsync(string idToken)
  {
    string? jwks = null;
    JwtSecurityTokenHandler jwtHandler = new();
    JwtSecurityToken jwt = jwtHandler.ReadJwtToken(idToken);

    JwtHeader header = jwt.Header;
    string kid = header.Kid;
    string alg = header.Alg;

    // 1️⃣ Fetch Apple’s public keys
    // check the cache first
    var cacheResponse = await dataService.GetCachedItemAsync<ApplePublicKeysCache>(
      ApplePublicKeys,
      ApplePublicKeys,
      ApplePublicKeys,
      nameof(ValidateAppleIdTokenAsync)
    );

    if (cacheResponse is null)
    {
      jwks = await httpClient.GetStringAsync("https://appleid.apple.com/auth/keys");
      if (string.IsNullOrEmpty(jwks))
      {
        throw new SecurityTokenException("Apple Public Keys could not be obtained.");
      }
      else
      {
        await dataService.CacheItemAsync(
          new ApplePublicKeysCache(jwks: jwks),
          ApplePublicKeys,
          ApplePublicKeys,
          nameof(ValidateAppleIdTokenAsync)
        );
      }
    }
    else
    {
      jwks = cacheResponse.jwks;
    }

    List<JObject> keys = JObject.Parse(jwks)["keys"]!.ToObject<List<JObject>>()!;

    JObject? matchingKey = keys.FirstOrDefault(k => k["kid"]!.ToString() == kid && k["alg"]!.ToString() == alg);
    if (matchingKey == null)
      throw new SecurityTokenException("No matching key found in Apple JWKS");

    // 2️⃣ Construct RSA key
    byte[] n = Base64UrlEncoder.DecodeBytes(matchingKey["n"]!.ToString());
    byte[] e = Base64UrlEncoder.DecodeBytes(matchingKey["e"]!.ToString());
    RSAParameters rsa = new() { Modulus = n, Exponent = e };
    RsaSecurityKey rsaKey = new(rsa) { KeyId = kid };

    // 3️⃣ Validate signature and claims
    TokenValidationParameters parameters = new()
    {
      ValidateIssuer = true,
      ValidIssuer = "https://appleid.apple.com",
      ValidateAudience = true,
      ValidAudience = audience,
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = rsaKey,
      ValidateLifetime = true,
      ClockSkew = TimeSpan.FromMinutes(2)
    };

    jwtHandler.ValidateToken(idToken, parameters, out var validatedToken);

    return (JwtSecurityToken)validatedToken;
  }
}

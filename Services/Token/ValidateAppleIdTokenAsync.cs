using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace ScripturAI.Services;

public partial class TokenService
{
  internal async Task<JwtSecurityToken> ValidateAppleIdTokenAsync(string idToken)
  {
    var jwtHandler = new JwtSecurityTokenHandler();
    var jwt = jwtHandler.ReadJwtToken(idToken);

    var header = jwt.Header;
    var kid = header.Kid;
    var alg = header.Alg;

    // 1️⃣ Fetch Apple’s public keys
    var jwks = await httpClient.GetStringAsync("https://appleid.apple.com/auth/keys");
    /** 
      * ToDo: Cache Apple public keys for 24hrs (use Cosmos cache)

      private static (DateTime fetchedAt, string json)? cachedJwks;

      var jwks = cachedJwks?.json;
      if (jwks == null || (DateTime.UtcNow - cachedJwks.Value.fetchedAt).TotalHours > 24)
      {
          jwks = await http.GetStringAsync("https://appleid.apple.com/auth/keys");
          cachedJwks = (DateTime.UtcNow, jwks);
      }
    */
    var keys = JObject.Parse(jwks)["keys"]!.ToObject<List<JObject>>()!;

    var matchingKey = keys.FirstOrDefault(k => k["kid"]!.ToString() == kid && k["alg"]!.ToString() == alg);
    if (matchingKey == null)
      throw new SecurityTokenException("No matching key found in Apple JWKS");

    // 2️⃣ Construct RSA key
    var n = Base64UrlEncoder.DecodeBytes(matchingKey["n"]!.ToString());
    var e = Base64UrlEncoder.DecodeBytes(matchingKey["e"]!.ToString());
    var rsa = new RSAParameters { Modulus = n, Exponent = e };
    var rsaKey = new RsaSecurityKey(rsa) { KeyId = kid };

    // 3️⃣ Validate signature and claims
    var parameters = new TokenValidationParameters
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

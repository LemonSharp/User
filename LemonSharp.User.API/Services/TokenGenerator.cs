using LemonSharp.User.API.JWT;
using Microsoft.Extensions.Options;
using SparkTodo.API.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace LemonSharp.User.API.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly TokenOptions _tokenOptions;

    public TokenGenerator(IOptions<TokenOptions> tokenOptions) => _tokenOptions = tokenOptions.Value;

    public TokenEntity GenerateToken(params Claim[] claims)
    {
        var now = DateTime.UtcNow;
        var claimList = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
        };
        if (claims != null)
        {
            claimList.AddRange(claims);
        }

        // Create the JWT and write it to a string
        var jwt = new JwtSecurityToken(
            issuer: _tokenOptions.Issuer,
            audience: _tokenOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: now.Add(_tokenOptions.ValidFor),
            signingCredentials: _tokenOptions.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler()
            .WriteToken(jwt);

        var response = new TokenEntity
        {
            AccessToken = encodedJwt,
            ExpiresIn = (int)_tokenOptions.ValidFor.TotalSeconds
        };
        return response;
    }

    /// <summary>
    /// ToUnixEpochDate
    /// </summary>
    /// <param name="date">DateTime</param>
    /// <returns></returns>
    public static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}

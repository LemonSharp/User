namespace LemonSharp.User.API.JWT;
/// <summary>
/// Token
/// </summary>
public class TokenEntity
{
    /// <summary>
    /// AccessToken
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// ExpiresIn
    /// </summary>
    public int ExpiresIn { get; set; }
}

public class UserTokenEntity : TokenEntity
{
    public string UserId { get; set; }

    public string[] UserRoles { get; set; }
}

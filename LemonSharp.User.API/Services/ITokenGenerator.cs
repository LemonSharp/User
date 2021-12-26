using System.Security.Claims;
using LemonSharp.User.API.JWT;

namespace LemonSharp.User.API.Services;

public interface ITokenGenerator
{
    /// <summary>
    /// 生成 Token
    /// </summary>
    /// <param name="claims">claims</param>
    /// <returns>token</returns>
    TokenEntity GenerateToken(params Claim[] claims);
}

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LemonSharp.User.API.DataAccess;
using LemonSharp.User.API.JWT;
using LemonSharp.User.API.Models;
using LemonSharp.User.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

namespace LemonSharp.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserIdentityDbContext _userContext;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenGenerator _tokenGenerator;

    public AccountController(UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        UserIdentityDbContext userContext,
        ITokenGenerator tokenGenerator)
    {
        _userContext = userContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenGenerator = tokenGenerator;
    }
    
    [Route("SignIn")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignInAsync([FromBody] LoginViewModel loginModel)
    {
        JsonResponseModel result;
        var signinResult = await _signInManager.PasswordSignInAsync(loginModel.PhoneNumer, loginModel.Password, true, lockoutOnFailure: false);
        if (signinResult.Succeeded)
        {
            var userInfo = await _userContext.Users.AsNoTracking().SingleAsync(x=>x.PhoneNumber == loginModel.PhoneNumer);
            var roles = await _userContext.UserRoles.AsNoTracking()
                .Where(x => x.UserId == userInfo.Id)
                .Join(_userContext.Roles.AsNoTracking(), x=>x.RoleId, r=>r.Id, (ur, r)=> r.Name)
                .ToArrayAsync();
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, GuidIdGenerator.Instance.NewId()),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id.ToString()),
            }.Union(roles.Select(x=> new Claim("role", x))).ToArray();
            var token = _tokenGenerator.GenerateToken(claims);

            var userToken = new UserTokenEntity
            {
                AccessToken = token.AccessToken,
                ExpiresIn = token.ExpiresIn,
                UserId = userInfo.Id,
            };
            result = new JsonResponseModel<TokenEntity> { Data = userToken, Msg = "", Status = JsonResponseStatus.Success };
        }
        else
        {
            if (signinResult.IsLockedOut)
            {
                result = new JsonResponseModel<TokenEntity> { Msg = "账户被锁定", Status = JsonResponseStatus.RequestError };
            }
            else
            {
                result = new JsonResponseModel<TokenEntity> { Msg = "登录失败", Status = JsonResponseStatus.AuthFail };
            }
        }
        return Ok(result);
    }

    [Route("SignUp")]
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> SignUpAsync([FromBody] RegisterViewModel regModel)
    {
        var userInfo = new IdentityUser()
        {
            UserName = regModel.PhoneNumber,
            PhoneNumber = regModel.PhoneNumber,
            PhoneNumberConfirmed = true,
            Email = $"{regModel.PhoneNumber}@lemonsharp.top",
            EmailConfirmed = true,
        };
        JsonResponseModel<TokenEntity> result;
        var signUpResult = await _userManager.CreateAsync(userInfo, regModel.Password);
        if (signUpResult.Succeeded)
        {
            userInfo = await _userContext.Users.SingleAsync(x=>x.PhoneNumber == regModel.PhoneNumber);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, GuidIdGenerator.Instance.NewId()),
                new Claim(JwtRegisteredClaimNames.Sub, userInfo.Id),
                new Claim(JwtRegisteredClaimNames.NameId, userInfo.Id),
            };
            var token = _tokenGenerator.GenerateToken(claims);

            var userToken = new UserTokenEntity
            {
                AccessToken = token.AccessToken,
                ExpiresIn = token.ExpiresIn,
                UserId = userInfo.Id,
            };
            result = new JsonResponseModel<TokenEntity> { Data = userToken, Msg = "", Status = JsonResponseStatus.Success };
        }
        else
        {
            result = new JsonResponseModel<TokenEntity> { Msg = "sign up failed," + string.Join(",", signUpResult.Errors.Select(e => e.Description).ToArray()), Status = JsonResponseStatus.ProcessFail };
        }
        return Ok(result);
    }
}

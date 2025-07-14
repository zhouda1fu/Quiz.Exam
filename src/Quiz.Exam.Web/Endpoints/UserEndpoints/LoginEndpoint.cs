using Azure.Core;
using Consul;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Jwt;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Configuration;
using Quiz.Exam.Web.Helper;
using System.Security.Claims;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token,string RefreshToken, UserId UserId, string Name, string Email);

[Tags("Users")]
[HttpPost("/api/user/login")]
[AllowAnonymous]
public class LoginEndpoint : Endpoint<LoginRequest, ResponseData<LoginResponse>>
{
    private readonly IMediator _mediator;
    private readonly UserQuery _userQuery;
    private readonly IJwtProvider _jwtProvider;
    private readonly IOptions<AppConfiguration> _appConfiguration;

    public LoginEndpoint(IMediator mediator, UserQuery userQuery, IJwtProvider jwtProvider, IOptions<AppConfiguration> appConfiguration)
    {
        _mediator = mediator;
        _userQuery = userQuery;
        _jwtProvider = jwtProvider;
        _appConfiguration = appConfiguration;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // 1. 查询：验证用户凭据
        var loginInfo = await _userQuery.GetUserInfoForLoginAsync(req.Username, ct) ?? throw new KnownException("无效的用户");
        var passwordHash = PasswordHasher.HashPassword(req.Password);

        if (passwordHash != loginInfo.PasswordHash)
            throw new KnownException("用户名或密码错误");


        // 2. 命令：更新用户登录时间
        var updateCmd = new UpdateUserLoginTimeCommand(loginInfo.UserId, DateTimeOffset.UtcNow);
        await _mediator.Send(updateCmd, ct);

        // 3. 生成JWT令牌

        var refreshToken = TokenGenerator.GenerateRefreshToken();
        var tokenExpiryTime = DateTimeOffset.Now.AddMinutes(_appConfiguration.Value.TokenExpiryInMinutes);
        var claims = new List<Claim>
        {
            new Claim("name", loginInfo.Name),
            new Claim("email", loginInfo.Email),
            new Claim("sub", loginInfo.UserId.ToString()),
            new Claim("user_id", loginInfo.UserId.ToString())
        };

        var token = await _jwtProvider.GenerateJwtToken(
            new JwtData("issuer-x", "audience-y", claims, DateTimeOffset.Now.DateTime, tokenExpiryTime.DateTime));

        var response = new LoginResponse(
            token,
            refreshToken,
            loginInfo.UserId,
            loginInfo.Name,
            loginInfo.Email
        );
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
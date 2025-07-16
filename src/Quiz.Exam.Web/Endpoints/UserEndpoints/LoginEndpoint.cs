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
using Quiz.Exam.Web.Const;
using Quiz.Exam.Web.Helper;
using System.Security.Claims;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token,string RefreshToken, UserId UserId, string Name, string Email);

[Tags("Users")]
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

    public override void Configure()
    {
        Post("/api/user/login");
        AllowAnonymous();
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {

       
        // 1. 查询：验证用户凭据
        var loginInfo = await _userQuery.GetUserInfoForLoginAsync(req.Username, ct) ?? throw new KnownException("无效的用户");
        var passwordHash = PasswordHasher.HashPassword(req.Password);


        if (!PasswordHasher.VerifyHashedPassword(req.Password, loginInfo.PasswordHash))
            throw new KnownException("用户名或密码错误");


        // 2. 命令：更新用户登录时间
        var updateCmd = new UpdateUserLoginTimeCommand(loginInfo.UserId, DateTimeOffset.UtcNow);
        await _mediator.Send(updateCmd, ct);

        // 3. 生成JWT令牌

        var refreshToken = TokenGenerator.GenerateRefreshToken();
        var nowTime = DateTimeOffset.Now;
        var tokenExpiryTime = nowTime.AddMinutes(_appConfiguration.Value.TokenExpiryInMinutes);
        var assignedPermissionCode = await _userQuery.GetAssignedPermissionCode(loginInfo.UserId, ct);
        var claims = new List<Claim>
        {
            new Claim("name", loginInfo.Name),
            new Claim("email", loginInfo.Email),
            new Claim("sub", loginInfo.UserId.ToString()),
            new Claim("user_id", loginInfo.UserId.ToString())
        };

        // 添加权限到 claims，FastEndpoints 会自动处理权限验证
        if (assignedPermissionCode != null)
        {
            foreach (var permissionCode in assignedPermissionCode)
            {
                claims.Add(new Claim("permissions", permissionCode));
            }
        }

        // 使用 FastEndpoints 的 JWT 生成方式
        var token = await _jwtProvider.GenerateJwtToken(
            new JwtData("issuer-x", "audience-y", claims, nowTime.DateTime, tokenExpiryTime.DateTime));



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
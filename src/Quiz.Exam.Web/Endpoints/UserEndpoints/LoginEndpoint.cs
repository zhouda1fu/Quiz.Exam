using FastEndpoints;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Queries;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Jwt;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Security.Claims;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, UserId UserId, string Username, string Email);

[Tags("Users")]
[HttpPost("/api/user/login")]
[AllowAnonymous]
public class LoginEndpoint : Endpoint<LoginRequest, ResponseData<LoginResponse>>
{
    private readonly IMediator _mediator;
    private readonly UserQuery _userQuery;
    private readonly IJwtProvider _jwtProvider;

    public LoginEndpoint(IMediator mediator, UserQuery userQuery, IJwtProvider jwtProvider)
    {
        _mediator = mediator;
        _userQuery = userQuery;
        _jwtProvider = jwtProvider;
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        // 1. 查询：验证用户凭据
        var loginInfo = await _userQuery.ValidateUserLoginAsync(req.Username, req.Password, ct);
        if (loginInfo == null)
        {
            throw new KnownException("用户名或密码错误");
        }

        // 2. 命令：更新用户登录时间
        var updateCmd = new UpdateUserLoginTimeCommand(loginInfo.UserId,DateTimeOffset.UtcNow);
        await _mediator.Send(updateCmd, ct);

        // 3. 生成JWT令牌
        var claims = new List<Claim>
        {
            new Claim("name", loginInfo.Username),
            new Claim("email", loginInfo.Email),
            new Claim("sub", loginInfo.UserId.ToString()),
            new Claim("user_id", loginInfo.UserId.ToString())
        };

        var jwt = await _jwtProvider.GenerateJwtToken(
            new JwtData("netcorepal", "netcorepal", claims, DateTime.Now, DateTime.Now.AddDays(1)));

        var response = new LoginResponse(
            jwt,
            loginInfo.UserId,
            loginInfo.Username,
            loginInfo.Email
        );
        
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
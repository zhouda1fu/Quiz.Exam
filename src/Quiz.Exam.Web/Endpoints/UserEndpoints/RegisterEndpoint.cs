using FastEndpoints;
using Quiz.Exam.Web.Application.Commands;
using NetCorePal.Extensions.Dto;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record RegisterRequest(string Username, string Email, string Password);

public record RegisterResponse(UserId UserId, string Username, string Email);

[Tags("Users")]
[HttpPost("/api/user/register")]
[AllowAnonymous]
public class RegisterEndpoint : Endpoint<RegisterRequest, ResponseData<RegisterResponse>>
{
    private readonly IMediator _mediator;

    public RegisterEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        // 使用 MediatR 处理注册逻辑
        var cmd = new CreateUserCommand(req.Username, req.Email, req.Password);
        var userId = await _mediator.Send(cmd, ct);
        
        var response = new RegisterResponse(
            userId,
            req.Username,
            req.Email
        );
        
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
} 
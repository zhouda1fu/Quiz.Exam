using Azure.Core;
using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record RegisterRequest(string Name, string Email, string Password, string Phone, string RealName, int Status, IEnumerable<RoleId> RoleIds);

public record RegisterResponse(UserId UserId, string Name, string Email);

[Tags("Users")]
public class RegisterEndpoint : Endpoint<RegisterRequest, ResponseData<RegisterResponse>>
{
    private readonly IMediator _mediator;
    private readonly RoleQuery _roleQuery;

    public RegisterEndpoint(IMediator mediator, RoleQuery roleQuery)
    {
        _mediator = mediator;
        _roleQuery = roleQuery;
    }

    public override void Configure()
    {
        Post("/api/user/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterRequest request, CancellationToken ct)
    {

        var rolesToBeAssigned = await _roleQuery.GetAdminRolesForAssignmentAsync(request.RoleIds, ct);

        // 哈希密码
        var passwordHash = PasswordHasher.HashPassword(request.Password);
        var cmd = new CreateUserCommand(request.Name, request.Email, passwordHash, request.Phone, request.RealName, request.Status, rolesToBeAssigned);
        var userId = await _mediator.Send(cmd, ct);
        var response = new RegisterResponse(
            userId,
            request.Name,
            request.Email
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
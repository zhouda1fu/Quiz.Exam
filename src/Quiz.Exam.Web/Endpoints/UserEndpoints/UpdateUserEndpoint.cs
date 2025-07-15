using Azure.Core;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Const;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;


public record UpdateUserRequest(UserId UserId, string Name, string Email, string Password, string Phone, string RealName, int Status);

public record UpdateUserResponse(UserId UserId, string Name, string Email);

[Tags("Users")]
public class UpdateUserEndpoint : Endpoint<UpdateUserRequest, ResponseData<UpdateUserResponse>>
{

    private readonly IMediator _mediator;
    private readonly RoleQuery _roleQuery;

    public UpdateUserEndpoint(IMediator mediator, RoleQuery roleQuery)
    {
        _mediator = mediator;
        _roleQuery = roleQuery;
    }

    public override void Configure()
    {
        Put("/api/user/update");
        AuthSchemes("Bearer");
        Permissions(AppPermissions.UserUpdate);
    }

    public override async Task HandleAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var passwordHash = PasswordHasher.HashPassword(request.Password);
        var cmd = new UpdateUserCommand(request.UserId, request.Name, request.Email, passwordHash, request.Phone, request.RealName, request.Status);
        var userId = await _mediator.Send(cmd, ct);
        var response = new UpdateUserResponse(
            userId,
            request.Name,
            request.Email
        );
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
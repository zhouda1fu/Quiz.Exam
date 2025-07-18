using Azure.Core;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.AppPermissions;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;


public record UpdateUserRequest(UserId UserId, string Name, string Email, string Phone, string RealName, int Status);

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
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        // FastEndpoints 会自动从 JWT claims 中验证权限
        Permissions(PermissionCodes.UserEdit);
    }

    public override async Task HandleAsync(UpdateUserRequest request, CancellationToken ct)
    {
        //var passwordHash = PasswordHasher.HashPassword(request.Password);
        var cmd = new UpdateUserCommand(request.UserId, request.Name, request.Email, request.Phone, request.RealName, request.Status);
        var userId = await _mediator.Send(cmd, ct);
        var response = new UpdateUserResponse(
            userId,
            request.Name,
            request.Email
        );
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
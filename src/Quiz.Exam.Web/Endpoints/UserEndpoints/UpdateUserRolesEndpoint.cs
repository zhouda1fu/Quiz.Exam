using Azure.Core;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.UserCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;


public record UpdateUserRolesRequest(UserId UserId, IEnumerable<RoleId> RoleIds);

public record UpdateUserRolesResponse(UserId UserId);

[Tags("Users")]
[Authorize(AuthenticationSchemes = "Bearer")]
[HttpPut("/api/user/update_roles")]
public class UpdateUserRolesEndpoint : Endpoint<UpdateUserRolesRequest,ResponseData<UpdateUserRolesResponse>>
{

    private readonly IMediator _mediator;
    private readonly RoleQuery _roleQuery;

    public UpdateUserRolesEndpoint(IMediator mediator, RoleQuery roleQuery)
    {
        _mediator = mediator;
        _roleQuery = roleQuery;
    }

    public override async Task HandleAsync(UpdateUserRolesRequest request, CancellationToken ct)
    {
        var rolesToBeAssigned = await _roleQuery.GetAdminRolesForAssignmentAsync(request.RoleIds, ct);

        var cmd = new UpdateUserRolesCommand(request.UserId, rolesToBeAssigned);
        await _mediator.Send(cmd, ct);
        var response = new UpdateUserRolesResponse(
            request.UserId
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
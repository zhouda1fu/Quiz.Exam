using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Web.Application.Commands.RoleCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Const;
using Quiz.Exam.Web.Endpoints.UserEndpoints;
using static FastEndpoints.Ep;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record UpdateRoleInfoRequest(RoleId RoleId, string Name, string Description, bool IsActive,    IEnumerable<string> PermissionCodes);

[Tags("Roles")]
[HttpPut("/api/roles/{roleId}")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class UpdateRoleEndpoint : Endpoint<UpdateRoleInfoRequest, ResponseData<bool>>
{
    private readonly RoleQuery _roleQuery;
    private readonly IMediator _mediator;

    public UpdateRoleEndpoint(RoleQuery roleQuery, IMediator mediator)
    {
        _roleQuery = roleQuery;
        _mediator = mediator;
    }

    public override async Task HandleAsync(UpdateRoleInfoRequest request,CancellationToken ct)
    {
        var cmd = new UpdateRoleInfoCommand(request.RoleId,request.Name,request.Description,request.IsActive,request.PermissionCodes);
        await _mediator.Send(cmd, ct);
        await SendAsync(true.AsResponseData(), cancellation: ct);
    }
} 
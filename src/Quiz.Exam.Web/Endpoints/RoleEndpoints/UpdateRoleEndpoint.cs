using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Web.Application.Commands.RoleCommands;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.AppPermissions;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record UpdateRoleInfoRequest(RoleId RoleId, string Name, string Description, IEnumerable<string> PermissionCodes);

[Tags("Roles")]
public class UpdateRoleEndpoint : Endpoint<UpdateRoleInfoRequest, ResponseData<bool>>
{
    private readonly IMediator _mediator;

    public UpdateRoleEndpoint(RoleQuery roleQuery, IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/api/roles/update");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.RoleCreate);
    }

    public override async Task HandleAsync(UpdateRoleInfoRequest request, CancellationToken ct)
    {
        var cmd = new UpdateRoleInfoCommand(request.RoleId, request.Name, request.Description,  request.PermissionCodes);
        await _mediator.Send(cmd, ct);
        await SendAsync(true.AsResponseData(), cancellation: ct);
    }
}
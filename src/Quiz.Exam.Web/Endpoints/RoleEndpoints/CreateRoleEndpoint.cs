using FastEndpoints;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Web.Application.Commands;
using Quiz.Exam.Web.Application.Commands.RoleCommands;
using Quiz.Exam.Web.AppPermissions;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record CreateRoleRequest(string Name, string Description, IEnumerable<string> PermissionCodes);

public record CreateRoleResponse(string RoleId, string Name, string Description);

[Tags("Roles")]
public class CreateRoleEndpoint : Endpoint<CreateRoleRequest, ResponseData<CreateRoleResponse>>
{
    private readonly IMediator _mediator;

    public CreateRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/api/roles");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.RoleCreate);
    }

    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
    {
        var cmd = new CreateRoleCommand(req.Name, req.Description,req.PermissionCodes);
        var result = await _mediator.Send(cmd, ct);
        
        var response = new CreateRoleResponse(
            result.ToString(),
            req.Name,
            req.Description
        );
        
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
} 
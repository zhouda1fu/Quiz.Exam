using FastEndpoints;
using Quiz.Exam.Web.Application.Commands;
using NetCorePal.Extensions.Dto;
using Microsoft.AspNetCore.Authorization;
using MediatR;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record CreateRoleRequest(string Name, string Description, IEnumerable<string> PermissionCodes);

public record CreateRoleResponse(string RoleId, string Name, string Description);

[Tags("Roles")]
[HttpPost("/api/roles")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class CreateRoleEndpoint : Endpoint<CreateRoleRequest, ResponseData<CreateRoleResponse>>
{
    private readonly IMediator _mediator;

    public CreateRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
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
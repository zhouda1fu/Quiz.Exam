using FastEndpoints;
using Quiz.Exam.Web.Application.Queries;
using NetCorePal.Extensions.Dto;
using Microsoft.AspNetCore.Authorization;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record GetActiveRolesResponse(string Id, string Name, string Description, bool IsActive, DateTimeOffset CreatedTime);

[Tags("Roles")]
[HttpGet("/api/roles/active")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class GetActiveRolesEndpoint : EndpointWithoutRequest<ResponseData<IEnumerable<GetActiveRolesResponse>>>
{
    private readonly RoleQuery _roleQuery;

    public GetActiveRolesEndpoint(RoleQuery roleQuery)
    {
        _roleQuery = roleQuery;
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roles = await _roleQuery.GetActiveRolesAsync(ct);
        
        var response = roles.Select(r => new GetActiveRolesResponse(
            r.Id.ToString(),
            r.Name,
            r.Description,
            r.IsActive,
            r.CreatedTime
        ));
        
        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
} 
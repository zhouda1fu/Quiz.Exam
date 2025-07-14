using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Queries;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record GetRoleRequest(RoleId RoleId);

public record GetRoleResponse(string Id, string Name, string Description, bool IsActive, DateTimeOffset CreatedTime);

[Tags("Roles")]
[HttpGet("/api/roles/{roleId}")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class GetRoleEndpoint : Endpoint<GetRoleRequest, ResponseData<GetRoleResponse?>>
{
    private readonly RoleQuery _roleQuery;

    public GetRoleEndpoint(RoleQuery roleQuery)
    {
        _roleQuery = roleQuery;
    }

    public override async Task HandleAsync(GetRoleRequest req, CancellationToken ct)
    {
       
        
        var roleInfo = await _roleQuery.GetRoleByIdAsync(req.RoleId, ct) ?? throw new KnownException("Invalid Credentials.");
        var response = new GetRoleResponse(
            roleInfo.Id.ToString(),
            roleInfo.Name,
            roleInfo.Description,
            roleInfo.IsActive,
            roleInfo.CreatedTime
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);

    }
} 
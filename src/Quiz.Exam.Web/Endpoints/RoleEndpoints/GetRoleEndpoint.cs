using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.AppPermissions;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record GetRoleRequest(RoleId RoleId);

public record GetRoleResponse(RoleId Id, string Name, string Description, bool IsActive, DateTimeOffset CreatedAt);

[Tags("Roles")]
public class GetRoleEndpoint : Endpoint<GetRoleRequest, ResponseData<GetRoleResponse?>>
{
    private readonly RoleQuery _roleQuery;

    public GetRoleEndpoint(RoleQuery roleQuery)
    {
        _roleQuery = roleQuery;
    }

    public override void Configure()
    {
        Get("/api/roles/{roleId}");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.RoleView);
    }

    public override async Task HandleAsync(GetRoleRequest req, CancellationToken ct)
    {
        var roleInfo = await _roleQuery.GetRoleByIdAsync(req.RoleId, ct) ?? throw new KnownException("Invalid Credentials.");
        var response = new GetRoleResponse(
            roleInfo.RoleId,
            roleInfo.Name,
            roleInfo.Description,
            roleInfo.IsActive,
            roleInfo.CreatedAt
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
} 
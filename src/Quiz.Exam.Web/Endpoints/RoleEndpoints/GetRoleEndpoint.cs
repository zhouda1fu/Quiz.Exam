using FastEndpoints;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Const;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

public record GetRoleRequest(RoleId RoleId);

public record GetRoleResponse(string Id, string Name, string Description, bool IsActive, DateTimeOffset CreatedTime);

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
        AuthSchemes("Bearer");
        Permissions(AppPermissions.RoleRead);
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
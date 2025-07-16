using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Const;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

[Tags("Roles")]
public class GetAllRoleEndpoint : Endpoint<RoleQueryInput, ResponseData<PagedData<RoleInfo>?>>
{
    private readonly RoleQuery _roleQuery;

    public GetAllRoleEndpoint(RoleQuery roleQuery)
    {
        _roleQuery = roleQuery;
    }

    public override void Configure()
    {
        Get("/api/roles");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(AppPermissions.RoleList);
    }

    public override async Task HandleAsync(RoleQueryInput req, CancellationToken ct)
    {
        var roleInfo = await _roleQuery.GetAllRolesAsync(req, ct) ?? throw new KnownException("Invalid Credentials.");
        await SendAsync(roleInfo.AsResponseData(), cancellation: ct);
    }
} 
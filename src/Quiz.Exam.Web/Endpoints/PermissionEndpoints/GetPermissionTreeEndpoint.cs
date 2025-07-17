using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.AppPermissions;
using System.Security;

namespace Quiz.Exam.Web.Endpoints.RoleEndpoints;

[Tags("Permissions")]
public class GetPermissionTreeEndpoint : EndpointWithoutRequest<ResponseData<IEnumerable<AppPermissionGroup>>>
{
    public override void Configure()
    {
        Get("/api/permissions/tree");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.UserRoleAssign);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var PermissionGroups = PermissionDefinitionContext.PermissionGroups;
        await SendAsync(new ResponseData<IEnumerable<AppPermissionGroup>>(PermissionGroups), cancellation: ct);  
    }
} 
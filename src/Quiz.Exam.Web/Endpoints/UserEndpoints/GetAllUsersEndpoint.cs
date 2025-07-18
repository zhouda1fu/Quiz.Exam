using FastEndpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.AppPermissions;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public class GetAllUsersRequest : PageRequest
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
}

public record GetAllUsersResponse(UserInfoQueryDto[] Items, int Total, int PageIndex, int PageSize);

[Tags("Users")]
public class GetAllUsersEndpoint : Endpoint<GetAllUsersRequest, ResponseData<GetAllUsersResponse>>
{
    private readonly UserQuery _userQuery;

    public GetAllUsersEndpoint(UserQuery userQuery)
    {
        _userQuery = userQuery;
    }

    public override void Configure()
    {
        Get("/api/users");
        AuthSchemes(JwtBearerDefaults.AuthenticationScheme);
        Permissions(PermissionCodes.UserView);
    }

    public override async Task HandleAsync(GetAllUsersRequest req, CancellationToken ct)
    {
        var queryInput = new UserQueryInput
        {
            PageIndex = req.PageIndex,
            PageSize = req.PageSize,
            Keyword = req.Keyword,
            Status = req.Status,
            CountTotal = req.CountTotal
        };

        var result = await _userQuery.GetAllUsersAsync(queryInput, ct);

        var response = new GetAllUsersResponse(
            result.Items.ToArray(),
            result.Total,
            result.PageIndex,
            result.PageSize
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
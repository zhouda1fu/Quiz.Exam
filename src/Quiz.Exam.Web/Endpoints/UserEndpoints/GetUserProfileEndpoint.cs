using FastEndpoints;
using NetCorePal.Extensions.Dto;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Const;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record GetUserProfileRequest(UserId UserId);

public record UserProfileResponse(UserId UserId, string Name, string Phone, IEnumerable<string> Roles, string RealName, int Status, string Email, DateTimeOffset CreatedAt);

[Tags("Users")]
public class GetUserProfileEndpoint : Endpoint<GetUserProfileRequest, ResponseData<UserProfileResponse?>>
{
    private readonly UserQuery _userQuery;

    public GetUserProfileEndpoint(UserQuery userQuery)
    {
        _userQuery = userQuery;
    }

    public override void Configure()
    {
        Get("/api/user/profile/{userId}");
        AuthSchemes("Bearer");
        Permissions(AppPermissions.UserRead);
    }

    public override async Task HandleAsync(GetUserProfileRequest req, CancellationToken ct)
    {
        var userInfo = await _userQuery.GetUserByIdAsync(req.UserId, ct);

        if (userInfo == null)
        {
            throw new KnownException("无效的用户");
        }

        var response = new UserProfileResponse(
            userInfo.UserId,
            userInfo.Name,
            userInfo.Phone,
            userInfo.Roles,
            userInfo.RealName,
            userInfo.Status,
            userInfo.Email,
            userInfo.CreatedAt
        );

        await SendAsync(response.AsResponseData(), cancellation: ct);
    }
}
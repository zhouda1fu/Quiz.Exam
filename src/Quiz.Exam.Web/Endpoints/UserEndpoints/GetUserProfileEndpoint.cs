using FastEndpoints;
using Quiz.Exam.Web.Application.Queries;
using NetCorePal.Extensions.Dto;
using Microsoft.AspNetCore.Authorization;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;

namespace Quiz.Exam.Web.Endpoints.UserEndpoints;

public record GetUserProfileRequest(UserId UserId);

public record UserProfileResponse(UserId UserId, string Name, string Phone, IEnumerable<string> Roles, string RealName, int Status, string Email, DateTimeOffset CreatedAt);

[Tags("Users")]
[HttpGet("/api/user/profile/{userId}")]
[Authorize(AuthenticationSchemes = "Bearer")]
public class GetUserProfileEndpoint : Endpoint<GetUserProfileRequest, ResponseData<UserProfileResponse?>>
{
    private readonly UserQuery _userQuery;

    public GetUserProfileEndpoint(UserQuery userQuery)
    {
        _userQuery = userQuery;
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
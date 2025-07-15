using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Application.Queries;

public record UserInfo(UserId UserId, string Name, string Phone, IEnumerable<string> Roles, string RealName, int Status, string Email, DateTimeOffset CreatedAt);

public record UserLoginInfo(UserId UserId, string Name, string Email, string PasswordHash);



public class UserQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<User> UserSet { get; } = applicationDbContext.Users;

    public async Task<bool> DoesUserExist(string username, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .AnyAsync(u => u.Name == username, cancellationToken: cancellationToken);
    }

    public async Task<bool> DoesEmailExist(string email, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<UserInfo?> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(au => new UserInfo(au.Id, au.Name, au.Phone, au.Roles.Select(r => r.RoleName), au.RealName, au.Status, au.Email, au.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<List<string>> GetAssignedPermissionCode(UserId id,
    CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .Where(au => au.Id == id)
            .SelectMany(au => au.Permissions)
            .Select(aup => aup.PermissionCode)
            .ToListAsync(cancellationToken);
    }


    public async Task<List<UserId>> GetUserIdsByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Roles.Any(r => r.RoleId == roleId))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }


    public async Task<UserLoginInfo?> GetUserInfoForLoginAsync(string name, CancellationToken cancellationToken)
    {
        return await UserSet
        .Where(u => u.Name == name)
        .Select(u => new UserLoginInfo(u.Id, u.Name, u.Email, u.PasswordHash))
        .FirstOrDefaultAsync(cancellationToken);
    }

}
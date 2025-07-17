using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure;
using Quiz.Exam.Web.Const;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Application.Queries;

public record UserInfoQueryDto(UserId UserId, string Name, string Phone, IEnumerable<string> Roles, string RealName, int Status, string Email, DateTimeOffset CreatedAt);

public record UserLoginInfoQueryDto(UserId UserId, string Name, string Email, string PasswordHash);

public class UserQueryInput : PageRequest
{
    public string? Keyword { get; set; }
    public int? Status { get; set; }
}

public class UserQuery(ApplicationDbContext applicationDbContext, IMemoryCache memoryCache) : IQuery
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

    public async Task<UserInfoQueryDto?> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(au => new UserInfoQueryDto(au.Id, au.Name, au.Phone, au.Roles.Select(r => r.RoleName), au.RealName, au.Status, au.Email, au.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<string>?> GetAssignedPermissionCode(UserId id,
    CancellationToken cancellationToken)
    {
        var cacheKey = $"{CacheKeys.UserPermissions}:{id}";
        var userPermissions = await memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
            return await UserSet.AsNoTracking()
                .Where(au => au.Id == id)
                .SelectMany(au => au.Permissions)
                .Select(aup => aup.PermissionCode)
                .ToListAsync(cancellationToken);
        });

        return userPermissions;
    }

    public async Task<List<UserId>> GetUserIdsByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Roles.Any(r => r.RoleId == roleId))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserLoginInfoQueryDto?> GetUserInfoForLoginAsync(string name, CancellationToken cancellationToken)
    {
        return await UserSet
        .Where(u => u.Name == name)
        .Select(u => new UserLoginInfoQueryDto(u.Id, u.Name, u.Email, u.PasswordHash))
        .FirstOrDefaultAsync(cancellationToken);
    }

    /// <summary>
    /// 清除用户权限缓存
    /// </summary>
    /// <param name="userId">用户ID</param>
    public void ClearUserPermissionCache(UserId userId)
    {
        var cacheKey = $"{CacheKeys.UserPermissions}:{userId}";
        memoryCache.Remove(cacheKey);
    }

    public async Task<PagedData<UserInfoQueryDto>> GetAllUsersAsync(UserQueryInput query, CancellationToken cancellationToken)
    {
        var queryable = UserSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Keyword))
        {
            queryable = queryable.Where(u => u.Name.Contains(query.Keyword!) || u.Email.Contains(query.Keyword!));
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(u => u.Status == query.Status);
        }

        return await queryable
            .OrderByDescending(u => u.CreatedAt)
            .Where(u => !u.IsDeleted)
            .Select(u => new UserInfoQueryDto(
                u.Id,
                u.Name,
                u.Phone,
                u.Roles.Select(r => r.RoleName),
                u.RealName,
                u.Status,
                u.Email,
                u.CreatedAt))
            .ToPagedDataAsync(query, cancellationToken);
    }
}
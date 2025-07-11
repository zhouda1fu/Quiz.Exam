using Microsoft.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Queries;

public record UserInfo(UserId UserId, string Username, string Email, bool IsActive, DateTimeOffset CreatedTime, DateTimeOffset? LastLoginTime);

public record UserLoginInfo(UserId UserId, string Username, string Email);

public class UserQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<User> UserSet { get; } = applicationDbContext.Users;

    public async Task<bool> DoesUserExist(string username, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .AnyAsync(u => u.Username == username, cancellationToken: cancellationToken);
    }

    public async Task<bool> DoesEmailExist(string email, CancellationToken cancellationToken)
    {
        return await UserSet.AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken: cancellationToken);
    }

    public async Task<UserInfo?> GetUserByIdAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserInfo(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.CreatedTime,
                u.LastLoginTime))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<UserInfo?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => new UserInfo(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.CreatedTime,
                u.LastLoginTime))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserInfo>> GetActiveUsersAsync(CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.IsActive)
            .Select(u => new UserInfo(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.CreatedTime,
                u.LastLoginTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInfo>> GetUsersByRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        return await UserSet.AsNoTracking()
            .Where(u => u.Roles.Any(r => r.RoleName == roleName))
            .Select(u => new UserInfo(
                u.Id,
                u.Username,
                u.Email,
                u.IsActive,
                u.CreatedTime,
                u.LastLoginTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<UserLoginInfo?> ValidateUserLoginAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        var user = await UserSet
            .Where(u => u.Username == username)
            .Select(u => new { u.Id, u.Username, u.Email, u.PasswordHash, u.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        // 验证密码
        if (!VerifyPassword(password, user.PasswordHash))
        {
            return null;
        }

        return new UserLoginInfo(user.Id, user.Username, user.Email);
    }

    private bool VerifyPassword(string password, string passwordHash)
    {
        // 这里使用简单的哈希比较，实际项目中应使用 BCrypt 或其他安全的哈希算法
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        var hashString = Convert.ToBase64String(hashBytes);
        return hashString == passwordHash;
    }
} 
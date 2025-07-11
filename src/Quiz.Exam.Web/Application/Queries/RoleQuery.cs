using Microsoft.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Infrastructure;
using NetCorePal.Extensions.Primitives;

namespace Quiz.Exam.Web.Application.Queries;

public record RoleInfo(RoleId Id, string Name, string Description, bool IsActive, DateTimeOffset CreatedTime);

public class RoleQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Role> RoleSet { get; } = applicationDbContext.Roles;

    public async Task<bool> DoesRoleExist(string name, CancellationToken cancellationToken)
    {
        return await RoleSet.AsNoTracking()
            .AnyAsync(r => r.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<RoleInfo?> GetRoleByIdAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RoleInfo(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive,
                r.CreatedTime))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<RoleInfo?> GetRoleByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => r.Name == name)
            .Select(r => new RoleInfo(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive,
                r.CreatedTime))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<RoleInfo>> GetActiveRolesAsync(CancellationToken cancellationToken = default)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => r.IsActive)
            .Select(r => new RoleInfo(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive,
                r.CreatedTime))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<RoleInfo>> GetRolesByPermissionAsync(string permissionCode, CancellationToken cancellationToken = default)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => r.Permissions.Any(p => p.PermissionCode == permissionCode))
            .Select(r => new RoleInfo(
                r.Id,
                r.Name,
                r.Description,
                r.IsActive,
                r.CreatedTime))
            .ToListAsync(cancellationToken);
    }
} 
using Microsoft.EntityFrameworkCore;
using NetCorePal.Extensions.Dto;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Infrastructure;

namespace Quiz.Exam.Web.Application.Queries;

public record RoleQueryDto(RoleId RoleId, string Name, string Description, bool IsActive, DateTimeOffset CreatedTime, IEnumerable<string> PermissionCodes);

public class RoleQueryInput : PageRequest 
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public bool? IsActive { get; set; }
}

public record AssignAdminUserRoleQueryDto(RoleId RoleId, string RoleName, IEnumerable<string> PermissionCodes);

public class RoleQuery(ApplicationDbContext applicationDbContext) : IQuery
{
    private DbSet<Role> RoleSet { get; } = applicationDbContext.Roles;

    public async Task<bool> DoesRoleExist(string name, CancellationToken cancellationToken)
    {
        return await RoleSet.AsNoTracking()
            .AnyAsync(r => r.Name == name, cancellationToken: cancellationToken);
    }

    public async Task<List<AssignAdminUserRoleQueryDto>> GetAdminRolesForAssignmentAsync(IEnumerable<RoleId> ids,
    CancellationToken cancellationToken)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => ids.Contains(r.Id))
            .Select(r => new AssignAdminUserRoleQueryDto(
                r.Id,
                r.Name,
                r.Permissions.Select(rp => rp.PermissionCode)))
            .ToListAsync(cancellationToken);
    }


    public async Task<RoleQueryDto?> GetRoleByIdAsync(RoleId id, CancellationToken cancellationToken = default)
    {
        return await RoleSet.AsNoTracking()
            .Where(r => r.Id == id)
            .Select(r => new RoleQueryDto(r.Id, r.Name, r.Description, r.IsActive, r.CreatedAt, r.Permissions.Select(rp => rp.PermissionCode)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PagedData<RoleQueryDto>> GetAllRolesAsync(RoleQueryInput query,
    CancellationToken cancellationToken)
    {
        return await RoleSet.AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(query.Name), r => r.Name.Contains(query.Name!))
            .WhereIf(!string.IsNullOrWhiteSpace(query.Description), r => r.Description.Contains(query.Description!))
            .WhereIf(query.IsActive.HasValue, r => r.IsActive == query.IsActive)
            .OrderBy(r => r.Id)
            .Select(r => new RoleQueryDto(r.Id, r.Name,  r.Description, r.IsActive, r.CreatedAt, r.Permissions.Select(rp => rp.PermissionCode)))
            .ToPagedDataAsync(query, cancellationToken);
    }

} 
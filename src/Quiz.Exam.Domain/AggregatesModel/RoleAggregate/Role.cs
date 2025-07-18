using NetCorePal.Extensions.Domain;
using NetCorePal.Extensions.Primitives;
using Quiz.Exam.Domain.DomainEvents.RoleEvents;
using System.Net.NetworkInformation;

namespace Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

public partial record RoleId : IInt64StronglyTypedId 
{

    //强类型ID添加静态的TryParse方法，这样FastEndpoints就能自动将字符串路由参数转换为强类型ID。
    public static bool TryParse(string? input, out RoleId output)
    {
        output = new RoleId(0);
        if (string.IsNullOrEmpty(input))
        {
            return false;
        }

        if (long.TryParse(input, out var longValue))
        {
            output = new RoleId(longValue);
            return true;
        }

        return false;
    }
}

public class Role : Entity<RoleId>, IAggregateRoot
{
    protected Role()
    {
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; init; }
    public bool IsActive { get; private set; } = true;
    public bool IsDeleted { get; private set; } = false;
    public DateTimeOffset? DeletedTime { get; private set; }

    public virtual ICollection<RolePermission> Permissions { get; init; } = [];




    public Role(string name, string description, IEnumerable<RolePermission> permissions)
    {
        CreatedAt = DateTimeOffset.Now;
        Name = name;
        Description = description;
        Permissions = new List<RolePermission>(permissions);
        IsActive = true;
    }

    public void UpdateRoleInfo(string name, string description)
    {
        Name = name;
        Description = description;
        AddDomainEvent(new RoleInfoChangedDomainEvent(this));
    }

    public void UpdateRolePermissions(IEnumerable<RolePermission> newPermissions)
    {
        var currentPermissionMap = Permissions.ToDictionary(p => p.PermissionCode);
        var newPermissionMap = newPermissions.ToDictionary(p => p.PermissionCode);

        var permissionsToRemove = currentPermissionMap.Keys.Except(newPermissionMap.Keys).ToList();
        foreach (var permissionCode in permissionsToRemove)
        {
            Permissions.Remove(currentPermissionMap[permissionCode]);
        }

        var permissionsToAdd = newPermissionMap.Keys.Except(currentPermissionMap.Keys).ToList();
        foreach (var permissionCode in permissionsToAdd)
        {
            Permissions.Add(newPermissionMap[permissionCode]);
        }

        AddDomainEvent(new RolePermissionChangedDomainEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new KnownException("角色已经被停用");
        }

        IsActive = false;
    }

    public void Activate()
    {
        if (IsActive)
        {
            throw new KnownException("角色已经是激活状态");
        }

        IsActive = true;
    }

    public void Delete()
    {
        if (IsDeleted)
        {
            throw new KnownException("角色已经被删除");
        }

        IsDeleted = true;
        DeletedTime = DateTimeOffset.UtcNow;
    }

    public bool HasPermission(string permissionCode)
    {
        return Permissions.Any(p => p.PermissionCode == permissionCode);
    }
} 
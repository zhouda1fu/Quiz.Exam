using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

namespace Quiz.Exam.Domain.AggregatesModel.UserAggregate;

public class UserPermission
{
    protected UserPermission()
    {
    }

    public UserId UserId { get; private set; } = default!;
    public string PermissionCode { get; private set; } = string.Empty;
    public List<RoleId> SourceRoleIds { get; } = [];

    public UserPermission(string permissionCode, RoleId? sourceRoleId = null)
    {
        PermissionCode = permissionCode;
        if (sourceRoleId is not null)
        {
            SourceRoleIds.Add(sourceRoleId);
        }
    }

    public void AddSourceRoleId(RoleId roleId)
    {
        if (SourceRoleIds.Contains(roleId)) return;
        SourceRoleIds.Add(roleId);
    }
} 
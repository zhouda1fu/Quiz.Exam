using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

namespace Quiz.Exam.Domain.AggregatesModel.UserAggregate;

public class UserRole
{
    protected UserRole() { }

    public UserId UserId { get; private set; } = default!;
    public RoleId RoleId { get; private set; } = default!;
    public string RoleName { get; private set; } = string.Empty;

    public UserRole(RoleId roleId, string roleName)
    {
        RoleId = roleId;
        RoleName = roleName;
    }

    public void UpdateRoleInfo(string roleName)
    {
        RoleName = roleName;
    }
} 
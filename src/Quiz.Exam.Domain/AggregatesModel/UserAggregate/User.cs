using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;

namespace Quiz.Exam.Domain.AggregatesModel.UserAggregate
{
    public partial record UserId : IInt64StronglyTypedId;

    public class User : Entity<UserId>, IAggregateRoot
    {
        protected User()
        {
        }

        public User(string username, string email, string passwordHash)
        {
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            IsActive = true;
            CreatedTime = DateTimeOffset.UtcNow;
        }

        public string Username { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public DateTimeOffset CreatedTime { get; private set; }
        public DateTimeOffset? LastLoginTime { get; private set; }
        public UpdateTime UpdateTime { get; private set; } = new UpdateTime(DateTimeOffset.UtcNow);

        // 用户角色关系
        public virtual ICollection<UserRole> Roles { get; } = [];
        
        // 用户权限关系
        public virtual ICollection<UserPermission> Permissions { get; } = [];

        public void UpdateRoleInfo(RoleId roleId, string roleName)
        {
            var savedRole = Roles.FirstOrDefault(r => r.RoleId == roleId);
            savedRole?.UpdateRoleInfo(roleName);
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrEmpty(newPasswordHash))
            {
                throw new KnownException("密码不能为空");
            }

            PasswordHash = newPasswordHash;
            UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        }

        public void RecordLogin()
        {
            LastLoginTime = DateTimeOffset.UtcNow;
            UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        }

        public void UpdateLastLoginTime(DateTimeOffset loginTime)
        {
            LastLoginTime = loginTime;
            UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                throw new KnownException("用户已经被停用");
            }

            IsActive = false;
            UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        }

        public void Activate()
        {
            if (IsActive)
            {
                throw new KnownException("用户已经是激活状态");
            }

            IsActive = true;
            UpdateTime = new UpdateTime(DateTimeOffset.UtcNow);
        }

        // 角色管理方法
        public void UpdateRoles(IEnumerable<UserRole> rolesToBeAssigned, IEnumerable<UserPermission> permissions)
        {
            var currentRoleMap = Roles.ToDictionary(r => r.RoleId);
            var targetRoleMap = rolesToBeAssigned.ToDictionary(r => r.RoleId);

            var roleIdsToRemove = currentRoleMap.Keys.Except(targetRoleMap.Keys);
            foreach (var roleId in roleIdsToRemove)
            {
                Roles.Remove(currentRoleMap[roleId]);
                RemoveRolePermissions(roleId);
            }

            var roleIdsToAdd = targetRoleMap.Keys.Except(currentRoleMap.Keys);
            foreach (var roleId in roleIdsToAdd)
            {
                var targetRole = targetRoleMap[roleId];
                Roles.Add(targetRole);
            }

            AddPermissions(permissions);
        }

        public void UpdateRolePermissions(RoleId roleId, IEnumerable<UserPermission> newPermissions)
        {
            RemoveRolePermissions(roleId);
            AddPermissions(newPermissions);
        }

        private void AddPermissions(IEnumerable<UserPermission> permissions)
        {
            foreach (var permission in permissions)
            {
                var existedPermission = Permissions.SingleOrDefault(p => p.PermissionCode == permission.PermissionCode);
                if (existedPermission is not null)
                {
                    foreach (var permissionSourceRoleId in permission.SourceRoleIds)
                        existedPermission.AddSourceRoleId(permissionSourceRoleId);
                }
                else
                {
                    Permissions.Add(permission);
                }
            }
        }

        private void RemoveRolePermissions(RoleId roleId)
        {
            foreach (var permission in Permissions.Where(
                             p => p.SourceRoleIds.Remove(roleId) &&
                                  p.SourceRoleIds.Count == 0)
                         .ToArray())
            {
                Permissions.Remove(permission);
            }
        }

        public void SetSpecificPermissions(IEnumerable<UserPermission> permissionsToBeAssigned)
        {
            var currentSpecificPermissionMap =
                Permissions.Where(p => p.SourceRoleIds.Count == 0).ToDictionary(p => p.PermissionCode);
            var newSpecificPermissionMap = permissionsToBeAssigned.ToDictionary(p => p.PermissionCode);

            var permissionCodesToRemove = currentSpecificPermissionMap.Keys.Except(newSpecificPermissionMap.Keys);
            foreach (var permissionCode in permissionCodesToRemove)
            {
                var permission = currentSpecificPermissionMap[permissionCode];
                Permissions.Remove(permission);
            }

            var permissionCodesToAdd = newSpecificPermissionMap.Keys.Except(currentSpecificPermissionMap.Keys);
            foreach (var permissionCode in permissionCodesToAdd)
            {
                if (Permissions.Any(p => p.PermissionCode == permissionCode))
                    throw new KnownException("权限重复！");
                Permissions.Add(newSpecificPermissionMap[permissionCode]);
            }
        }

        public bool IsInRole(string roleName)
        {
            return Roles.Any(r => r.RoleName == roleName);
        }

        public bool HasPermission(string permissionCode)
        {
            return Permissions.Any(p => p.PermissionCode == permissionCode);
        }

        public IEnumerable<string> GetRoleNames()
        {
            return Roles.Select(r => r.RoleName);
        }

        public IEnumerable<string> GetPermissionCodes()
        {
            return Permissions.Select(p => p.PermissionCode);
        }
    }
} 
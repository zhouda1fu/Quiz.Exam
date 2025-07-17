namespace Quiz.Exam.Web.AppPermissions
{
    /// <summary>
    /// 权限常量定义
    /// </summary>
    public static class PermissionCodes
    {
        #region 角色管理权限
        public const string RoleManagement = nameof(RoleManagement);
        public const string RoleCreate = nameof(RoleCreate);
        public const string RoleEdit = nameof(RoleEdit);
        public const string RoleDelete = nameof(RoleDelete);
        public const string RoleView = nameof(RoleView);
        public const string RoleUpdatePermissions = nameof(RoleUpdatePermissions);
        #endregion

        #region 用户管理权限
        public const string UserManagement = nameof(UserManagement);
        public const string UserCreate = nameof(UserCreate);
        public const string UserEdit = nameof(UserEdit);
        public const string UserDelete = nameof(UserDelete);
        public const string UserView = nameof(UserView);
        public const string UserRoleAssign = nameof(UserRoleAssign);
        #endregion

        #region 系统管理权限
        public const string SystemAdmin = nameof(SystemAdmin);
        public const string SystemMonitor = nameof(SystemMonitor);
        #endregion
    }
} 
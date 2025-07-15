namespace Quiz.Exam.Web.Const
{
    /// <summary>
    /// 权限常量定义
    /// </summary>
    public static class AppPermissions
    {
        #region 角色管理权限
        public const string RoleCreate = nameof(RoleCreate);
        public const string RoleRead = nameof(RoleRead);
        public const string RoleUpdate = nameof(RoleUpdate);
        public const string RoleDelete = nameof(RoleDelete);
        public const string RoleList = nameof(RoleList);
        #endregion

        #region 用户管理权限
        public const string UserCreate = nameof(UserCreate);
        public const string UserRead = nameof(UserRead);
        public const string UserUpdate = nameof(UserUpdate);
        public const string UserDelete = nameof(UserDelete);
        public const string UserList = nameof(UserList);
        public const string UserLogin = nameof(UserLogin);
        public const string UserRegister = nameof(UserRegister);
        public const string UserRoleAssign = nameof(UserRoleAssign);
        #endregion

        #region 系统管理权限
        public const string SystemAdmin = nameof(SystemAdmin);
        public const string SystemMonitor = nameof(SystemMonitor);
        #endregion
    }
} 
﻿using System.Collections.Immutable;
using System.Security;

namespace Quiz.Exam.Web.AppPermissions
{
    /// <summary>
    /// 管理权限定义的上下文类，负责初始化和提供权限组及其权限项。
    /// </summary>
    public static class PermissionDefinitionContext
    {
        // 存储权限组的字典，键为权限组名称，值为权限组对象
        private static Dictionary<string, AppPermissionGroup> Groups { get; } = new();

        // 静态构造函数，在类初始化时创建默认的权限组和权限项
        static PermissionDefinitionContext()
        {
            var systemAccess = AddGroup("SystemAccess");
            var adminUserManagement = systemAccess.AddPermission(PermissionCodes.UserManagement, "用户管理");
            adminUserManagement.AddChild(PermissionCodes.UserCreate, "创建用户");
            adminUserManagement.AddChild(PermissionCodes.UserEdit, "编辑用户");
            adminUserManagement.AddChild(PermissionCodes.UserDelete, "删除用户");
            adminUserManagement.AddChild(PermissionCodes.UserView, "查看用户");
            adminUserManagement.AddChild(PermissionCodes.UserRoleAssign, "分配用户角色");
            var roleManagement = systemAccess.AddPermission(PermissionCodes.RoleManagement, "角色管理");
            roleManagement.AddChild(PermissionCodes.RoleCreate, "创建角色");
            roleManagement.AddChild(PermissionCodes.RoleEdit, "编辑角色");
            roleManagement.AddChild(PermissionCodes.RoleDelete, "删除角色");
            roleManagement.AddChild(PermissionCodes.RoleView, "查看角色");
            roleManagement.AddChild(PermissionCodes.RoleUpdatePermissions, "更新角色权限");

            //var systemAdmin = systemAccess.AddPermission(PermissionCodes.SystemAdmin, "系统管理员权限");
            var systemMonitor = systemAccess.AddPermission(PermissionCodes.SystemMonitor, "系统监控");

          
        }

        /// <summary>
        /// 添加一个新的权限组，如果权限组名称已存在则抛出异常。
        /// </summary>
        /// <param name="name">权限组名称</param>
        /// <returns>返回创建的权限组</returns>
        /// <exception cref="ArgumentException">如果权限组名称已经存在，则抛出异常</exception>
        private static AppPermissionGroup AddGroup(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            if (Groups.ContainsKey(name))
            {
                throw new ArgumentException($"There is already an existing permission group with name: {name}");
            }

            return Groups[name] = new AppPermissionGroup(name);
        }

        /// <summary>
        /// 获取所有的权限组。
        /// </summary>
        public static IReadOnlyList<AppPermissionGroup> PermissionGroups => Groups.Values.ToImmutableList();

        /// <summary>
        /// 获取所有的权限。
        /// </summary>
        public static IReadOnlyList<AppPermission> AllPermissions
        {
            get
            {
                if (_allPermissions is not null) return _allPermissions;
                var allPermissions = PermissionGroups.SelectMany(pg => pg.PermissionsWithChildren).ToImmutableList();
                _allPermissions = allPermissions;
                return _allPermissions;
            }
        }

        private static IReadOnlyList<AppPermission>? _allPermissions;

        /// <summary>
        /// 根据权限码获取对应的权限。如果权限不存在，抛出异常。
        /// </summary>
        /// <param name="code">权限码</param>
        /// <returns>返回对应的权限</returns>
        /// <exception cref="KnownException">如果未找到权限，则抛出异常</exception>
        public static AppPermission GetPermission(string code)
        {
            return AllPermissions.SingleOrDefault(p => p.Code == code) ??
                   throw new KnownException($"Permission with code '{code}' was not found");
        }
    }
}

﻿using System.Collections.Immutable;
using System.Security;

namespace Quiz.Exam.Web.AppPermissions
{
    /// <summary>
    /// 表示一个权限组，包含该组的所有权限以及相关操作。
    /// </summary>
    public sealed class AppPermissionGroup
    {
        /// <summary>
        /// 权限组的唯一名称。
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 权限组内的所有权限，权限是只读的。
        /// </summary>
        public IReadOnlyList<AppPermission> Permissions => _permissions.ToImmutableList();

        private readonly List<AppPermission> _permissions = [];

        /// <summary>
        /// 创建一个新的权限组。
        /// </summary>
        /// <param name="name">权限组的名称。</param>
        internal AppPermissionGroup(string name)
        {
            // 初始化权限组的名称和权限集合
            Name = name;
        }

        /// <summary>
        /// 向权限组中添加一个权限。
        /// </summary>
        /// <param name="code">权限的唯一代码。</param>
        /// <param name="name">权限的名称。</param>
        /// <param name="isEnabled">是否启用该权限，默认为 true。</param>
        /// <returns>返回创建的权限对象。</returns>
        public AppPermission AddPermission(string code, string name, bool isEnabled = true)
        {
            var permission = new AppPermission(code, name, isEnabled);
            _permissions.Add(permission); // 将权限添加到权限组中
            return permission;
        }


        private List<AppPermission>? _permissionsWithChildren;

        /// <summary>
        /// 当前权限组中的所有权限，包括子权限。
        /// </summary>
        public IReadOnlyList<AppPermission> PermissionsWithChildren
        {
            get
            {
                if (_permissionsWithChildren is not null)
                {
                    return _permissionsWithChildren;
                }

                var permissions = new List<AppPermission>();

                foreach (var permission in _permissions)
                {
                    AddPermissionToListRecursively(permissions, permission);
                }

                _permissionsWithChildren = permissions;
                return permissions.ToImmutableList();
            }
        }

        /// <summary>
        /// 递归地将权限及其子权限添加到列表中。
        /// </summary>
        /// <param name="permissions">目标权限列表。</param>
        /// <param name="permission">当前权限。</param>
        private static void AddPermissionToListRecursively(List<AppPermission> permissions, AppPermission permission)
        {
            permissions.Add(permission);

            foreach (var child in permission.Children)
            {
                AddPermissionToListRecursively(permissions, child);
            }
        }
    }
}

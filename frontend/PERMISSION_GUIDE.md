# 权限系统使用指南

## 概述

本系统实现了基于角色的权限控制（RBAC），支持菜单权限、页面权限和功能权限的控制。权限信息在用户登录时从后端获取，并存储在本地，支持页面刷新后权限状态的保持。

## 权限配置

### 1. 菜单权限配置

在 `src/stores/permission.ts` 中配置菜单权限：

```typescript
const menuPermissions: MenuPermission[] = [
  {
    path: '/',
    name: '仪表盘',
    icon: 'House',
    requiredPermissions: ['SystemMonitor']
  },
  {
    path: '/users',
    name: '用户管理',
    icon: 'User',
    requiredPermissions: ['UserView', 'UserManagement']
  },
  {
    path: '/roles',
    name: '角色管理',
    icon: 'Setting',
    requiredPermissions: ['RoleView', 'RoleManagement']
  }
]
```

### 2. 路由权限配置

在 `src/router/index.ts` 中为路由添加权限元数据：

```typescript
{
  path: 'users',
  name: 'Users',
  component: () => import('@/views/Users.vue'),
  meta: { permissions: ['UserView', 'UserManagement'] }
}
```

## 使用方法

### 1. 权限指令 (v-permission)

在模板中使用权限指令控制元素显示：

```vue
<template>
  <!-- 单个权限 -->
  <el-button v-permission="['UserCreate']">新建用户</el-button>
  
  <!-- 多个权限（需要满足所有权限） -->
  <el-button v-permission="['UserEdit', 'UserView']">编辑用户</el-button>
</template>
```

### 2. 权限组件 (Permission)

使用权限组件包装需要权限控制的内容：

```vue
<template>
  <Permission :permissions="['UserCreate']">
    <el-button type="primary">新建用户</el-button>
  </Permission>
  
  <!-- 满足任一权限即可 -->
  <Permission :permissions="['UserEdit', 'UserView']" :any="true">
    <el-button>编辑用户</el-button>
  </Permission>
</template>
```

### 3. 权限工具函数

在脚本中使用权限工具函数：

```typescript
import { hasPermission, hasAnyPermission, isSystemAdmin } from '@/utils/permission'

// 检查是否有权限
if (hasPermission(['UserCreate'])) {
  // 有创建用户权限
}

// 检查是否有任一权限
if (hasAnyPermission(['UserEdit', 'UserView'])) {
  // 有编辑或查看权限
}

// 检查是否是系统管理员
if (isSystemAdmin()) {
  // 是系统管理员
}
```

### 4. 在 Store 中使用

```typescript
import { usePermissionStore } from '@/stores/permission'

const permissionStore = usePermissionStore()

// 检查权限
const canCreateUser = permissionStore.hasPermission(['UserCreate'])

// 获取有权限的菜单
const authorizedMenus = permissionStore.getAuthorizedMenus
```

## 权限代码说明

### 用户管理权限
- `UserManagement`: 用户管理总权限
- `UserCreate`: 创建用户
- `UserEdit`: 编辑用户
- `UserDelete`: 删除用户
- `UserView`: 查看用户
- `UserRoleAssign`: 分配用户角色

### 角色管理权限
- `RoleManagement`: 角色管理总权限
- `RoleCreate`: 创建角色
- `RoleEdit`: 编辑角色
- `RoleDelete`: 删除角色
- `RoleView`: 查看角色
- `RoleUpdatePermissions`: 更新角色权限

### 系统权限
- `SystemAdmin`: 系统管理员（拥有所有权限）
- `SystemMonitor`: 系统监控

## 权限检查逻辑

1. **系统管理员权限**: 拥有 `SystemAdmin` 权限的用户自动拥有所有权限
2. **菜单权限**: 用户需要拥有菜单配置中的任一权限才能看到菜单
3. **页面权限**: 用户需要拥有路由配置中的所有权限才能访问页面
4. **功能权限**: 用户需要拥有对应的权限才能使用功能

## 最佳实践

1. **权限粒度**: 建议将权限设置得足够细，便于精确控制
2. **权限组合**: 使用多个权限组合来控制复杂功能
3. **默认权限**: 为常用功能设置合理的默认权限
4. **权限缓存**: 系统会自动缓存用户权限，提高性能
5. **权限验证**: 前端权限控制主要用于用户体验，后端仍需进行权限验证

## 注意事项

1. 前端权限控制主要用于提升用户体验，不能替代后端权限验证
2. 权限信息在登录时获取，权限变更后需要重新登录才能生效
3. 权限信息会保存在本地存储中，页面刷新后仍可保持
4. 系统管理员拥有所有权限，无需单独配置
5. 权限代码需要与后端保持一致
6. 如果用户没有任何权限，系统会自动跳转到登录页面 
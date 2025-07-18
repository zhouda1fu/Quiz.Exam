using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.AppPermissions;
using Quiz.Exam.Web.Helper;

namespace Quiz.Exam.Web.Extensions;

public static class SeedDatabaseExtension
{
    internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 初始化角色和权限
        if (!dbContext.Roles.Any())
        {
            var adminPermissions = new List<RolePermission>
            {
                new RolePermission(PermissionCodes.UserCreate, "创建用户", "创建新用户"),
                new RolePermission(PermissionCodes.UserView, "查看用户", "查看用户信息"),
                new RolePermission(PermissionCodes.UserEdit, "更新用户", "更新用户信息"),
                new RolePermission(PermissionCodes.UserDelete, "删除用户", "删除用户"),
                new RolePermission(PermissionCodes.RoleCreate, "创建角色", "创建新角色"),
                new RolePermission(PermissionCodes.RoleView, "查看角色", "查看角色信息"),
                new RolePermission(PermissionCodes.RoleEdit, "更新角色", "更新角色信息"),
                new RolePermission(PermissionCodes.RoleDelete, "删除角色", "删除角色"),
                new RolePermission(PermissionCodes.UserRoleAssign, "分配用户角色", "分配用户角色权限"),
                new RolePermission(PermissionCodes.RoleUpdatePermissions, "更新角色权限", "更新角色的权限"),
                //new RolePermission(PermissionCodes.SystemAdmin, "系统管理员权限", "拥有系统管理员权限"),
                new RolePermission(PermissionCodes.SystemMonitor, "系统监控权限", "拥有系统监控权限")

            };
            
            var userPermissions = new List<RolePermission>
            {
                new RolePermission(PermissionCodes.UserView, "查看用户", "查看用户信息"),
                new RolePermission(PermissionCodes.UserEdit, "更新用户", "更新自己的用户信息")
            };
            
            var adminRole = new Role("管理员", "系统管理员", adminPermissions);
            var userRole = new Role("普通用户", "普通用户", userPermissions);
            
            dbContext.Roles.Add(adminRole);
            dbContext.Roles.Add(userRole);
            dbContext.SaveChanges();
        }

        // 初始化管理员用户
        if (!dbContext.Users.Any(u => u.Name == "admin"))
        {
            var adminRole = dbContext.Roles.First(r => r.Name == "管理员");
            var adminUser = new User(
                "admin",
                "13800138000",
                PasswordHasher.HashPassword("123456"),
                new List<UserRole> { new UserRole(adminRole.Id, adminRole.Name) },
                new List<UserPermission> { 
                    new UserPermission(PermissionCodes.UserEdit, adminRole.Id),
                    new UserPermission(PermissionCodes.UserView, adminRole.Id),
                    new UserPermission(PermissionCodes.UserCreate, adminRole.Id),
                    new UserPermission(PermissionCodes.UserDelete, adminRole.Id),
                    new UserPermission(PermissionCodes.RoleCreate, adminRole.Id),
                    new UserPermission(PermissionCodes.RoleView, adminRole.Id),
                    new UserPermission(PermissionCodes.RoleEdit, adminRole.Id),
                    new UserPermission(PermissionCodes.RoleDelete, adminRole.Id),
                    new UserPermission(PermissionCodes.UserRoleAssign, adminRole.Id),
                    //new UserPermission(PermissionCodes.SystemAdmin, adminRole.Id),
                    new UserPermission(PermissionCodes.SystemMonitor, adminRole.Id),
                },
                "系统管理员",
                1,
                "admin@example.com"
            );
            
            dbContext.Users.Add(adminUser);
            dbContext.SaveChanges();
        }

        // 初始化测试用户
        if (!dbContext.Users.Any(u => u.Name == "test"))
        {
            var userRole = dbContext.Roles.First(r => r.Name == "普通用户");
            var testUser = new User(
                "test",
                "13800138001",
                PasswordHasher.HashPassword("123456"),
                new List<UserRole> { new UserRole(userRole.Id, userRole.Name) },
                new List<UserPermission> { new UserPermission(PermissionCodes.UserEdit, userRole.Id) },
                "测试用户",
                1,
                "test@example.com"
            );
            
            dbContext.Users.Add(testUser);
            dbContext.SaveChanges();
        }

        return app;
    }
} 
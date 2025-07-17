using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Web.Const;
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
                new RolePermission(AppPermissions.UserCreate, "创建用户", "创建新用户"),
                new RolePermission(AppPermissions.UserRead, "查看用户", "查看用户信息"),
                new RolePermission(AppPermissions.UserUpdate, "更新用户", "更新用户信息"),
                new RolePermission(AppPermissions.UserDelete, "删除用户", "删除用户"),
                new RolePermission(AppPermissions.UserList, "用户列表", "查看用户列表"),
                new RolePermission(AppPermissions.RoleCreate, "创建角色", "创建新角色"),
                new RolePermission(AppPermissions.RoleRead, "查看角色", "查看角色信息"),
                new RolePermission(AppPermissions.RoleUpdate, "更新角色", "更新角色信息"),
                new RolePermission(AppPermissions.RoleDelete, "删除角色", "删除角色"),
                new RolePermission(AppPermissions.RoleList, "角色列表", "查看角色列表")
            };
            
            var userPermissions = new List<RolePermission>
            {
                new RolePermission(AppPermissions.UserRead, "查看用户", "查看用户信息"),
                new RolePermission(AppPermissions.UserUpdate, "更新用户", "更新自己的用户信息")
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
                    new UserPermission(AppPermissions.UserUpdate, adminRole.Id),
                    new UserPermission(AppPermissions.UserRead, adminRole.Id),
                    new UserPermission(AppPermissions.UserList, adminRole.Id),
                    new UserPermission(AppPermissions.UserCreate, adminRole.Id),
                    new UserPermission(AppPermissions.UserDelete, adminRole.Id),
                    new UserPermission(AppPermissions.RoleCreate, adminRole.Id),
                    new UserPermission(AppPermissions.RoleRead, adminRole.Id),
                    new UserPermission(AppPermissions.RoleUpdate, adminRole.Id),
                    new UserPermission(AppPermissions.RoleDelete, adminRole.Id),
                    new UserPermission(AppPermissions.RoleList, adminRole.Id),
                    new UserPermission(AppPermissions.UserRoleAssign, adminRole.Id),
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
                new List<UserPermission> { new UserPermission(AppPermissions.UserUpdate, userRole.Id) },
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
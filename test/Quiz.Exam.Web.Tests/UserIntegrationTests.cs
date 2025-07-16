using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Quiz.Exam.Domain.AggregatesModel.RoleAggregate;
using Quiz.Exam.Domain.AggregatesModel.UserAggregate;
using Quiz.Exam.Infrastructure;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Helper;
using Xunit;

namespace Quiz.Exam.Web.Tests;

[Collection("web")]
public class UserIntegrationTests : IClassFixture<MyWebApplicationFactory>
{
    private readonly MyWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UserIntegrationTests(MyWebApplicationFactory factory)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        
        _factory = factory;
        _client = factory.CreateClient();
    }

    /// <summary>
    /// 创建测试用户
    /// </summary>
    private async Task<UserId> CreateTestUser(string name, IEnumerable<RoleId> roleIds)
    {
        // 创建用户请求
        var createUserRequest = new
        {
            Name = name,
            Password = "123456",
            Phone = "13800138000",
            Email = $"{name}@example.com",
            RealName = name,
            RoleIds = roleIds
        };

        // 创建用户
        var response = await _client.PostAsJsonAsync("/api/user/register", createUserRequest);
        Assert.True(response.IsSuccessStatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<ResponseData<UserId>>();
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Data.Id);

        return responseData.Data;
    }

    /// <summary>
    /// 创建测试角色
    /// </summary>
    private async Task<RoleId> CreateTestRole(string roleName, string description, IEnumerable<string> permissionCodes)
    {
        var createRoleRequest = new
        {
            Name = roleName,
            Description = description,
            Permissions = permissionCodes
        };

        var response = await _client.PostAsJsonAsync("/api/role/create", createRoleRequest);
        Assert.True(response.IsSuccessStatusCode);
        
        var responseData = await response.Content.ReadFromJsonAsync<ResponseData<RoleId>>();
        Assert.NotNull(responseData);

        return responseData.Data;
    }

    /// <summary>
    /// 用户登录测试
    /// </summary>
    [Fact]
    public async Task UserLogin_ShouldSucceed()
    {
        // Arrange
        const string testUserName = "TestLoginUser";
        const string testPassword = "123456";
        
        await CreateTestUser(testUserName, []);

        // Act
        var loginRequest = new { Username = testUserName, Password = testPassword };
        var response = await _client.PostAsJsonAsync("/api/user/login", loginRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var loginResponse = await response.Content.ReadFromJsonAsync<ResponseData<LoginResponse>>();
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Data.Token);
        Assert.Equal(testUserName, loginResponse.Data.Name);
    }

    /// <summary>
    /// 用户注册测试
    /// </summary>
    [Fact]
    public async Task UserRegister_ShouldSucceed()
    {
        // Arrange
        var registerRequest = new
        {
            Name = "TestRegisterUser",
            Password = "123456",
            Phone = "13800138001",
            Email = "test@example.com",
            RealName = "测试用户"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/user/register", registerRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);
        var responseData = await response.Content.ReadFromJsonAsync<ResponseData<UserId>>();
        Assert.NotNull(responseData);
        Assert.NotEqual(0, responseData.Data.Id);
    }

    /// <summary>
    /// 用户角色管理测试
    /// </summary>
    [Fact]
    public async Task UserRoleManagement_ShouldSucceed()
    {
        // Arrange
        const string testUserName = "TestRoleUser";
        const string testRoleName = "TestRole";
        
        var permissionCodes = new[] { "user:read", "user:write" };
        var roleId = await CreateTestRole(testRoleName, "测试角色", permissionCodes);
        var userId = await CreateTestUser(testUserName, []);

        // Act - 分配角色
        var updateRolesRequest = new { RoleIds = new[] { roleId } };
        var response = await _client.PutAsJsonAsync($"/api/user/{userId}/roles", updateRolesRequest);

        // Assert
        Assert.True(response.IsSuccessStatusCode);

        // 验证用户角色
        var getUserResponse = await _client.GetAsync($"/api/user/{userId}");
        Assert.True(getUserResponse.IsSuccessStatusCode);
        
        var userInfo = await getUserResponse.Content.ReadFromJsonAsync<ResponseData<UserInfo>>();
        Assert.NotNull(userInfo);
        Assert.Contains(testRoleName, userInfo.Data.Roles);
    }

    /// <summary>
    /// 用户查询测试
    /// </summary>
    [Fact]
    public async Task UserQuery_ShouldSucceed()
    {
        // Arrange
        const string testUserName = "TestQueryUser";
        await CreateTestUser(testUserName, []);

        // Act - 查询所有用户
        var allUsersResponse = await _client.GetAsync("/api/user/all?PageIndex=1&PageSize=10");
        Assert.True(allUsersResponse.IsSuccessStatusCode);

        var allUsersData = await allUsersResponse.Content.ReadFromJsonAsync<ResponseData<PagedData<UserInfo>>>();
        Assert.NotNull(allUsersData);
        Assert.NotEmpty(allUsersData.Data.Items);

        // Act - 按条件查询用户
        var conditionResponse = await _client.GetAsync($"/api/user/all?Name={testUserName}&PageIndex=1");
        Assert.True(conditionResponse.IsSuccessStatusCode);
        
        var conditionData = await conditionResponse.Content.ReadFromJsonAsync<ResponseData<PagedData<UserInfo>>>();
        Assert.NotNull(conditionData);
        Assert.All(conditionData.Data.Items, user => Assert.Contains(testUserName, user.Name));
    }

    /// <summary>
    /// 健康检查测试
    /// </summary>
    [Fact]
    public async Task HealthCheck_ShouldSucceed()
    {
        var response = await _client.GetAsync("/health");
        Assert.True(response.IsSuccessStatusCode);
    }
}

// 辅助类
public record LoginResponse(string Token, string RefreshToken, UserId UserId, string Name, string Email);
public record UserInfo(UserId UserId, string Name, string Phone, IEnumerable<string> Roles, string RealName, int Status, string Email, DateTimeOffset CreatedAt);
public record PagedData<T>(IEnumerable<T> Items, int TotalCount, int PageIndex, int PageSize);
public record ResponseData<T>(T Data, int Code = 0, string Message = "Success");
public record ResponseData(int Code = 0, string Message = "Success"); 
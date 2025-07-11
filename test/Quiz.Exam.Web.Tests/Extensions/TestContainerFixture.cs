using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;

namespace Quiz.Exam.Web.Tests.Extensions;

public class TestContainerFixture : IDisposable
{
    public RedisContainer RedisContainer { get; } = new RedisBuilder()
        .WithCommand("--databases", "1024").Build();

    public RabbitMqContainer RabbitMqContainer { get; } = new RabbitMqBuilder()
        .WithUsername("guest").WithPassword("guest").Build();

    public MsSqlContainer MsSqlContainer { get; } = new MsSqlBuilder()
        .WithPassword("Test@123")
        .Build();

    public TestContainerFixture()
    {
        Task.WhenAll(RedisContainer.StartAsync(),
            RabbitMqContainer.StartAsync(),
            MsSqlContainer.StartAsync()).Wait();
    }

    public void Dispose()
    {
        Task.WhenAll(RedisContainer.StopAsync(),
            RabbitMqContainer.StopAsync(),
            MsSqlContainer.StopAsync()).Wait();
    }

    public async Task CreateVisualHostAsync(string visualHost)
    {
        await RabbitMqContainer.ExecAsync(new string[] { "rabbitmqctl", "add_vhost", visualHost });
        await RabbitMqContainer.ExecAsync(new string[]
            { "rabbitmqctl", "set_permissions", "-p", visualHost, "guest", ".*", ".*", ".*" });
    }
}
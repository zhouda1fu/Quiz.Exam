using FastEndpoints;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Prometheus;
using Quiz.Exam.Web.Application.Queries;
using Quiz.Exam.Web.Configuration;
using Quiz.Exam.Web.Extensions;
using Serilog;
using Serilog.Formatting.Json;
using StackExchange.Redis;
using System.Reflection;
using System.Text.Json;
using System.Linq;
#pragma warning disable S1118

public partial class Program
#pragma warning restore S1118
{
    private static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
    .Enrich.WithClientIp()
    .WriteTo.Console(new JsonFormatter())
    .CreateLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog();

            #region SignalR

            builder.Services.AddHealthChecks();
            builder.Services.AddMvc()
                .AddNewtonsoftJson(options => { options.SerializerSettings.AddNetCorePalJsonConverters(); });
            builder.Services.AddSignalR();

            #endregion

            #region Prometheus监控

            builder.Services.AddHealthChecks().ForwardToPrometheus();
            builder.Services.AddHttpClient(Options.DefaultName)
                .UseHttpClientMetrics();

            #endregion

            // Add services to the container.

            #region 身份认证

            var redis = await ConnectionMultiplexer.ConnectAsync(builder.Configuration.GetConnectionString("Redis")!);
            builder.Services.AddSingleton<IConnectionMultiplexer>(_ => redis);
            builder.Services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

            builder.Services.AddAuthentication().AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidAudience = "audience-y";
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidIssuer = "issuer-x";
                options.TokenValidationParameters.ValidateIssuer = true;
            });
            builder.Services.AddNetCorePalJwt().AddRedisStore();

            // 添加内存缓存
            builder.Services.AddMemoryCache();

            #endregion

            #region CORS

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            #endregion

            #region Controller

            builder.Services.AddControllers().AddNetCorePalSystemTextJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => c.AddEntityIdSchemaMap()); //强类型id swagger schema 映射

            #endregion

            #region FastEndpoints

            builder.Services.AddFastEndpoints();
            builder.Services.Configure<JsonOptions>(o =>
                o.SerializerOptions.AddNetCorePalJsonConverters());


            builder.Services.Configure<AppConfiguration>(builder.Configuration.GetSection("AppConfiguration"));


            #endregion

            #region 公共服务

            builder.Services.AddSingleton<IClock, SystemClock>();

            #endregion

            #region 集成事件

            // builder.Services.AddTransient<OrderPaidIntegrationEventHandler>();

            #endregion

            #region 模型验证器

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            builder.Services.AddKnownExceptionErrorModelInterceptor();

            #endregion

            #region Query

            builder.Services.AddScoped<UserQuery>();
            builder.Services.AddScoped<RoleQuery>();

            #endregion


            #region 基础设施

            builder.Services.AddRepositories(typeof(ApplicationDbContext).Assembly);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
                options.LogTo(Console.WriteLine, LogLevel.Information)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors();
            });
            builder.Services.AddUnitOfWork<ApplicationDbContext>();
            builder.Services.AddRedisLocks();
            builder.Services.AddContext().AddEnvContext().AddCapContextProcessor();
            builder.Services.AddNetCorePalServiceDiscoveryClient();
            builder.Services.AddIntegrationEvents(typeof(Program))
                .UseCap<ApplicationDbContext>(b =>
                {
                    b.RegisterServicesFromAssemblies(typeof(Program));
                    b.AddContextIntegrationFilters();
                    b.UseSqlServer();
                });


            builder.Services.AddCap(x =>
            {
                x.JsonSerializerOptions.AddNetCorePalJsonConverters();
                x.UseEntityFramework<ApplicationDbContext>();
                x.UseRabbitMQ(p => builder.Configuration.GetSection("RabbitMQ").Bind(p));
                x.UseDashboard(); //CAP Dashboard  path：  /cap
            });

            #endregion

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly())
                    .AddCommandLockBehavior()
                    .AddKnownExceptionValidationBehavior()
                    .AddUnitOfWorkBehaviors());

            #region 多环境支持与服务注册发现

            builder.Services.AddMultiEnv(envOption => envOption.ServiceName = "Abc.Template")
                .UseMicrosoftServiceDiscovery();
            builder.Services.AddConfigurationServiceEndpointProvider();
            builder.Services.AddEnvFixedConnectionChannelPool();

            #endregion

            #region 远程服务客户端配置

            //var jsonSerializerSettings = new JsonSerializerSettings
            //{
            //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
            //    NullValueHandling = NullValueHandling.Ignore,
            //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            //};
            //jsonSerializerSettings.AddNetCorePalJsonConverters();
            //var ser = new NewtonsoftJsonContentSerializer(jsonSerializerSettings);
            //var settings = new RefitSettings(ser);
            //builder.Services.AddRefitClient<IUserServiceClient>(settings)
            //    .ConfigureHttpClient(client =>
            //        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("https+http://user:8080")!))
            //    .AddMultiEnvMicrosoftServiceDiscovery() //多环境服务发现支持
            //    .AddStandardResilienceHandler(); //添加标准的重试策略

            #endregion

            #region Jobs

            builder.Services.AddHangfire(x => { x.UseRedisStorage(builder.Configuration.GetConnectionString("Redis")); });
            builder.Services.AddHangfireServer(); //hangfire dashboard  path：  /hangfire

            #endregion


            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await dbContext.Database.EnsureCreatedAsync();
                app.SeedDatabase();
            }

            app.UseKnownExceptionHandler();
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseRouting();

            // 添加 CORS 中间件
            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();



            app.MapControllers();
            app.UseFastEndpoints();

            #region SignalR

            app.MapHub<Quiz.Exam.Web.Application.Hubs.ChatHub>("/chat");

            #endregion

            app.UseHttpMetrics();
            app.MapHealthChecks("/health");
            app.MapMetrics("/metrics"); // 通过   /metrics  访问指标
            app.UseHangfireDashboard();
            await app.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}

#pragma warning disable S1118
public partial class Program
#pragma warning restore S1118
{
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;
using Testcontainers.Redis;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Modulith.NewModule.Infrastructure;
using System.Threading.Tasks;
using System;

namespace Modulith.NewModule.Tests.Integration;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove existing DbContext registration
            services.RemoveAll<DbContextOptions<ModulithNewModuleDbContext>>();
            services.RemoveAll<ModulithNewModuleDbContext>();

            // Add DbContext with Testcontainers PostgreSQL connection
            services.AddDbContext<ModulithNewModuleDbContext>(options =>
            {
                options.UseNpgsql(_postgreSqlContainer.GetConnectionString());
            });

            // Configure CAP to use Testcontainers RabbitMQ and PostgreSQL
            services.Configure<CapOptions>(capOptions =>
            {
                capOptions.UsePostgreSql(opts => opts.ConnectionString = _postgreSqlContainer.GetConnectionString());
                capOptions.UseRabbitMQ(opts => opts.ConnectionFactoryOptions = opt =>
                {
                    opt.HostName = _rabbitMqContainer.Hostname;
                    opt.Port = _rabbitMqContainer.GetPort();
                    opt.UserName = _rabbitMqContainer.Username;
                    opt.Password = _rabbitMqContainer.Password;
                });
                capOptions.UseRedisLock(opts => opts.ConnectionString = _redisContainer.GetConnectionString());
            });

            // Configure Redis cache to use Testcontainers Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = _redisContainer.GetConnectionString();
                options.InstanceName = "modulithtest:";
            });

            // Build service provider to ensure DbContext is created and migrations run
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<ModulithNewModuleDbContext>();
                dbContext.Database.Migrate();
            }
        });
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgreSqlContainer.DisposeAsync();
        await _rabbitMqContainer.DisposeAsync();
        await _redisContainer.DisposeAsync();
    }
} 
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Modulith.Template.Api.Workers;
using Modulith.Template.Domain.Interfaces;
using Modulith.Template.Infrastructure.Persistence;
using Modulith.Template.Infrastructure.Repositories;

namespace Modulith.Template.Api;

public static class TemplateModuleServiceRegistrar
{
    public static IServiceCollection AddTemplateModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<TemplateDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("Postgres"),
                b => b.MigrationsAssembly("Modulith.Template.Infrastructure")));

        // Register Repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Configure CAP
        services.AddCap(options =>
        {
            options.UseEntityFramework<TemplateDbContext>();
            options.UseRabbitMQ(options =>
            {
                options.HostName = configuration["RabbitMQ:HostName"];
                options.UserName = configuration["RabbitMQ:UserName"];
                options.Password = configuration["RabbitMQ:Password"];
                options.VirtualHost = configuration["RabbitMQ:VirtualHost"];
            });

            options.UseDashboard();
            options.FailedRetryCount = 3;
        });

        // Register Worker
        services.AddHostedService<TemplateWorker>();

        return services;
    }
} 
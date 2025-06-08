using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modulith.NewModule.Application.IntegrationEvents;
using Modulith.NewModule.Domain.Interfaces;
using Modulith.NewModule.Infrastructure.Data;
using Modulith.NewModule.Infrastructure.Repositories;
using System.Reflection;

namespace Modulith.NewModule.Api;

public static class NewModuleServiceRegistrar
{
    public static IServiceCollection AddNewModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<NewModuleDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("NewModuleDb"),
                b => b.MigrationsAssembly(typeof(NewModuleDbContext).Assembly.FullName)));

        // Configure CAP
        services.AddCap(options =>
        {
            options.UseEntityFramework<NewModuleDbContext>();
            options.UseRabbitMQ(options =>
            {
                options.HostName = configuration["RabbitMQ:HostName"];
                options.UserName = configuration["RabbitMQ:UserName"];
                options.Password = configuration["RabbitMQ:Password"];
                options.VirtualHost = configuration["RabbitMQ:VirtualHost"];
            });

            options.UseDashboard();
            options.FailedRetryCount = 3;
            options.FailedRetryInterval = 60;
        });

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();

        // Register MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        return services;
    }
} 
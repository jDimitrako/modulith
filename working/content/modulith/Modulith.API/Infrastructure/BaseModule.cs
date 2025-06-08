using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Modulith.API.Infrastructure;

public abstract class BaseModule : IModule
{
    protected readonly string ModuleName;
    protected readonly ModuleConfiguration Configuration;

    protected BaseModule(string moduleName, ModuleConfiguration configuration)
    {
        ModuleName = moduleName;
        Configuration = configuration;
    }

    public virtual void RegisterModule(IServiceCollection services, IConfiguration configuration)
    {
        // Register module-specific services
        RegisterServices(services, configuration);

        // Register module-specific health checks
        RegisterHealthChecks(services);

        // Register module-specific background jobs
        RegisterBackgroundJobs(services);

        // Register module-specific OpenTelemetry
        RegisterOpenTelemetry(services, configuration);
    }

    public virtual void UseModule(IApplicationBuilder app)
    {
        // Configure module-specific middleware
        UseMiddleware(app);

        // Configure module-specific endpoints
        UseEndpoints(app);
    }

    protected virtual void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Override in derived classes to register module-specific services
    }

    protected virtual void RegisterHealthChecks(IServiceCollection services)
    {
        // Override in derived classes to register module-specific health checks
    }

    protected virtual void RegisterBackgroundJobs(IServiceCollection services)
    {
        // Override in derived classes to register module-specific background jobs
    }

    protected virtual void RegisterOpenTelemetry(IServiceCollection services, IConfiguration configuration)
    {
        // Override in derived classes to register module-specific OpenTelemetry
    }

    protected virtual void UseMiddleware(IApplicationBuilder app)
    {
        // Override in derived classes to configure module-specific middleware
    }

    protected virtual void UseEndpoints(IApplicationBuilder app)
    {
        // Override in derived classes to configure module-specific endpoints
    }
} 
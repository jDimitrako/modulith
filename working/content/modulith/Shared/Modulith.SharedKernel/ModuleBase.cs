using Microsoft.Extensions.DependencyInjection;

namespace Modulith.SharedKernel;

public abstract class ModuleBase
{
    public abstract string ModuleName { get; }
    
    public virtual void RegisterModule(IServiceCollection services)
    {
        // Default implementation does nothing
        // Override in derived classes to register module-specific services
    }
    
    public virtual void UseModule(IApplicationBuilder app)
    {
        // Default implementation does nothing
        // Override in derived classes to configure module-specific middleware
    }
} 
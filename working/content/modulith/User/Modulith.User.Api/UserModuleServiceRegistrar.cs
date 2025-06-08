using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modulith.User.Contracts.Authorization;
using Modulith.User.Contracts.Options;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Authorization;
using Modulith.User.Infrastructure.Persistence;
using Modulith.User.Infrastructure.Services;

namespace Modulith.User.Api;

public static class UserModuleServiceRegistrar
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<UserDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));

        // Configure Identity
        services.AddIdentity<User, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = configuration.GetValue<bool>("UserModule:RequireEmailConfirmation");
        })
        .AddEntityFrameworkStores<UserDbContext>()
        .AddDefaultTokenProviders();

        // Configure external authentication
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"]!;
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = configuration["Authentication:Microsoft:ClientId"]!;
                options.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"]!;
            });

        // Configure authorization policies
        services.AddAuthorization(options =>
        {
            options.AddPolicy(UserPolicies.RequireAdminRole, policy =>
                policy.RequireRole("Admin"));

            options.AddPolicy(UserPolicies.RequireUserRole, policy =>
                policy.RequireRole("User"));

            options.AddPolicy(UserPolicies.RequireActiveUser, policy =>
                policy.AddRequirements(new ActiveUserRequirement()));

            options.AddPolicy(UserPolicies.RequireVerifiedEmail, policy =>
                policy.AddRequirements(new VerifiedEmailRequirement()));
        });

        // Register authorization handlers
        services.AddScoped<IAuthorizationHandler, ActiveUserHandler>();
        services.AddScoped<IAuthorizationHandler, VerifiedEmailHandler>();

        // Configure CAP
        services.AddCap(options =>
        {
            options.UseEntityFramework<UserDbContext>();
            options.UseRabbitMQ(options =>
            {
                options.HostName = configuration["RabbitMQ:HostName"] ?? "localhost";
                options.UserName = configuration["RabbitMQ:UserName"] ?? "guest";
                options.Password = configuration["RabbitMQ:Password"] ?? "guest";
                options.VirtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";
            });

            options.UseDashboard();
            options.FailedRetryCount = 3;
            options.FailedRetryInterval = 60;
        });

        // Register services
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailService, EmailService>();

        // Register MediatR
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(UserModuleServiceRegistrar).Assembly));

        // Configure options
        services.Configure<UserModuleOptions>(configuration.GetSection("UserModule"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
    }
} 
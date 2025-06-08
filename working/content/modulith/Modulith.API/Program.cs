using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modulith.SharedKernel.Infrastructure.OpenTelemetry;
using Modulith.SharedKernel.Infrastructure.RateLimiting;
using Modulith.SharedKernel.Infrastructure.BackgroundJobs;
using Modulith.SharedKernel.Infrastructure.Caching;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Modulith API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")))
        };
    });

// Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Postgres") ?? throw new InvalidOperationException("Postgres connection string not found"))
    .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? throw new InvalidOperationException("Redis connection string not found"));

// Add OpenTelemetry
builder.Services.AddSharedOpenTelemetry(
    builder.Configuration,
    "Modulith.API",
    "1.0.0");

// Add Rate Limiting
builder.Services.AddSharedRateLimiting(builder.Configuration);

// Add Background Jobs
builder.Services.AddSharedBackgroundJobs(builder.Configuration);

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Modulith_";
});

// Add Cache Service
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Register modules
// TODO: Add your module registrations here
// Example: NewModuleModuleServiceRegistrar.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add global exception handling
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

// Add health checks
app.UseHealthChecks("/health");

// Add rate limiting
app.UseRateLimiter();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Configure endpoints
app.MapControllers();

app.Run(); 
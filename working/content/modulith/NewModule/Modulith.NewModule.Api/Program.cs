using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Modulith.SharedKernel.Infrastructure.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddApiVersioningConfig();
builder.Services.AddHealthChecksConfig(builder.Configuration);
builder.Services.AddSharedOpenTelemetry(
    builder.Configuration,
    "Modulith.NewModule.Api",
    "1.0.0");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHealthChecksConfig();

if (!app.Environment.IsDevelopment())
{
    // The DeveloperExceptionPage is for development only. In production, GlobalExceptionHandlingMiddleware handles exceptions.
    // If you need more detailed error pages in production for specific errors, you'd add them here AFTER the GlobalExceptionHandlingMiddleware.
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run(); 
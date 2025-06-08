using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Modulith.SharedKernel.Infrastructure.RateLimiting;

public static class RateLimitingPolicies
{
    public static void AddRateLimitingPolicies(this RateLimiterOptions options)
    {
        // Global rate limiter (100 requests per minute per user/IP)
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // API endpoints rate limiter (1000 requests per minute per user/IP)
        options.AddPolicy("api", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 1000,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Authentication endpoints rate limiter (5 requests per minute per IP)
        options.AddPolicy("auth", httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 5,
                    Window = TimeSpan.FromMinutes(1)
                }));

        // Burst rate limiter (20 requests per 10 seconds per user/IP)
        options.AddPolicy("burst", httpContext =>
            RateLimitPartition.GetTokenBucketLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new TokenBucketRateLimiterOptions
                {
                    TokenLimit = 20,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2,
                    ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                    TokensPerPeriod = 20,
                    AutoReplenishment = true
                }));

        // Concurrent rate limiter (10 concurrent requests per user/IP)
        options.AddPolicy("concurrent", httpContext =>
            RateLimitPartition.GetConcurrencyLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new ConcurrencyLimiterOptions
                {
                    PermitLimit = 10,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = 2
                }));

        // Chunked rate limiter (1000 requests per hour per user/IP)
        options.AddPolicy("chunked", httpContext =>
            RateLimitPartition.GetChainedLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new ChainedRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 1000,
                    Window = TimeSpan.FromHours(1)
                }));

        // Custom rejection handler
        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                Error = "Too many requests. Please try again later.",
                RetryAfter = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter)
                    ? retryAfter.TotalSeconds
                    : null,
                Policy = context.PolicyName
            }, token);
        };
    }
} 
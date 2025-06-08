using Microsoft.AspNetCore.Http;
using Modulith.SharedKernel;

namespace Modulith.NewModule.Api;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    public const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, ICorrelationIdProvider correlationIdProvider)
    {
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();
        correlationIdProvider.SetCorrelationId(correlationId);
        context.Response.Headers[CorrelationIdHeader] = correlationId;
        await _next(context);
    }
} 
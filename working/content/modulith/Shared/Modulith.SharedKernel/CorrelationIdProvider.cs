using System.Threading;

namespace Modulith.SharedKernel;

public class CorrelationIdProvider : ICorrelationIdProvider
{
    private static readonly AsyncLocal<string?> _correlationId = new();

    public string GetCorrelationId() => _correlationId.Value ?? string.Empty;
    public void SetCorrelationId(string correlationId) => _correlationId.Value = correlationId;
} 
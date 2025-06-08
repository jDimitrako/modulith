namespace Modulith.SharedKernel;

public interface ICorrelationIdProvider
{
    string GetCorrelationId();
    void SetCorrelationId(string correlationId);
} 
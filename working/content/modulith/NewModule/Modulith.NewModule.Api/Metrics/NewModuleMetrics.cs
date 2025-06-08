using System.Diagnostics.Metrics;

namespace Modulith.NewModule.Api.Metrics;

public static class NewModuleMetrics
{
    private static readonly Meter Meter = new("Modulith.NewModule", "1.0.0");

    // Counters
    public static readonly Counter<int> UserCreatedCounter = Meter.CreateCounter<int>(
        "newmodule.users.created",
        "Users",
        "Number of users created in the NewModule");

    public static readonly Counter<int> UserUpdatedCounter = Meter.CreateCounter<int>(
        "newmodule.users.updated",
        "Users",
        "Number of users updated in the NewModule");

    // Histograms
    public static readonly Histogram<double> UserOperationDuration = Meter.CreateHistogram<double>(
        "newmodule.users.operation.duration",
        "Milliseconds",
        "Duration of user operations");

    // Gauges
    public static readonly ObservableGauge<int> ActiveUsersGauge = Meter.CreateObservableGauge(
        "newmodule.users.active",
        () => GetActiveUsersCount(),
        "Users",
        "Number of active users in the NewModule");

    private static int GetActiveUsersCount()
    {
        // Implement your logic to get the actual count of active users
        return 0;
    }
} 
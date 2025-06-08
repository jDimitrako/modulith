using OpenTelemetry.Trace;

namespace Modulith.NewModule.Api.OpenTelemetry;

public class OpenTelemetryExample
{
    private readonly ILogger<OpenTelemetryExample> _logger;
    private static readonly ActivitySource Activity = new("Modulith.NewModule.Api");

    public OpenTelemetryExample(ILogger<OpenTelemetryExample> logger)
    {
        _logger = logger;
    }

    public void ProcessOrder(string orderId, string product)
    {
        using var activity = Activity.StartActivity("ProcessOrder");
        activity?.SetTag("order.id", orderId);
        activity?.SetTag("order.product", product);

        _logger.LogInformation("Processing order {OrderId} for product {Product}", orderId, product);

        // Simulate some work
        Thread.Sleep(100);

        activity?.AddEvent(new ActivityEvent("OrderProcessed", tags: new ActivityTagsCollection { { "order.status", "completed" } }));

        _logger.LogInformation("Order {OrderId} processed successfully", orderId);
    }

    public void CalculatePrice(string productId, int quantity)
    {
        using var activity = Activity.StartActivity("CalculatePrice");
        activity?.SetTag("product.id", productId);
        activity?.SetTag("product.quantity", quantity);

        _logger.LogInformation("Calculating price for product {ProductId} with quantity {Quantity}", productId, quantity);

        // Simulate calculation
        Thread.Sleep(50);

        activity?.SetTag("price.calculated", true);
    }
} 
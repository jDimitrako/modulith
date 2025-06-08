using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modulith.NewModule.Api;
using Modulith.NewModule.Infrastructure.Data;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Modulith.NewModule.Tests.Integration;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> Factory;
    protected readonly HttpClient Client;
    protected readonly NewModuleDbContext DbContext;
    protected readonly JsonSerializerOptions JsonOptions;

    protected TestBase()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Replace the database with an in-memory database for testing
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<NewModuleDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<NewModuleDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("TestDb");
                    });

                    // Get the DbContext instance
                    var serviceProvider = services.BuildServiceProvider();
                    DbContext = serviceProvider.GetRequiredService<NewModuleDbContext>();
                });
            });

        Client = Factory.CreateClient();
        JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await Factory.DisposeAsync();
    }

    protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    protected StringContent SerializeRequest<T>(T request)
    {
        var json = JsonSerializer.Serialize(request, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    protected void SetAuthorizationHeader(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
} 
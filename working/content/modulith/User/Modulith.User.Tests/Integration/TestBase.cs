using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Modulith.User.Api;
using Modulith.User.Domain.Entities;
using Modulith.User.Infrastructure.Persistence;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Modulith.User.Tests.Integration;

public abstract class TestBase : IAsyncLifetime
{
    protected readonly WebApplicationFactory<Program> _factory;
    protected readonly HttpClient _client;
    protected readonly UserManager<User> _userManager;
    protected readonly UserDbContext _dbContext;
    protected readonly JsonSerializerOptions _jsonOptions;

    protected TestBase()
    {
        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.Testing.json", optional: false);
                });

                builder.ConfigureServices(services =>
                {
                    // Remove the existing DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<UserDbContext>));

                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    // Add in-memory database
                    services.AddDbContext<UserDbContext>(options =>
                    {
                        options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                    });

                    // Configure Identity to use in-memory database
                    services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.Password.RequireDigit = false;
                        options.Password.RequireLowercase = false;
                        options.Password.RequireUppercase = false;
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequiredLength = 6;
                    })
                    .AddEntityFrameworkStores<UserDbContext>()
                    .AddDefaultTokenProviders();
                });
            });

        _client = _factory.CreateClient();
        _userManager = _factory.Services.GetRequiredService<UserManager<User>>();
        _dbContext = _factory.Services.GetRequiredService<UserDbContext>();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task InitializeAsync()
    {
        await _dbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContext.Database.EnsureDeletedAsync();
        await _factory.DisposeAsync();
    }

    protected async Task<User> CreateTestUser(string email, string password, string role = "User")
    {
        var user = User.Create(email, "Test", "User");
        await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, role);
        return user;
    }

    protected async Task<string> GetAuthTokenAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        response.EnsureSuccessStatusCode();
        var loginResponse = await DeserializeResponseAsync<LoginResponse>(response);
        return loginResponse.Token;
    }

    protected void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<T?> DeserializeResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    protected async Task CleanupAsync()
    {
        _dbContext.Users.RemoveRange(_dbContext.Users);
        await _dbContext.SaveChangesAsync();
    }
} 
using Microsoft.AspNetCore.Identity;
using Modulith.User.Contracts.DTOs;
using Modulith.User.Domain.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Modulith.User.Tests.Integration;

public class ExternalAuthTests : TestBase
{
    private const string TestEmail = "test@example.com";
    private const string TestFirstName = "John";
    private const string TestLastName = "Doe";
    private const string TestProvider = "Google";
    private const string TestProviderKey = "123456789";

    [Fact]
    public async Task ExternalLogin_ValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new ExternalLoginRequest(
            Provider: TestProvider,
            ProviderKey: TestProviderKey,
            Email: TestEmail,
            FirstName: TestFirstName,
            LastName: TestLastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/external-login", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var loginResponse = await DeserializeResponseAsync<LoginResponse>(response);
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotNull(loginResponse.RefreshToken);

        var user = await _userManager.FindByEmailAsync(TestEmail);
        Assert.NotNull(user);
        Assert.Equal(TestEmail, user.Email);
        Assert.Equal(TestFirstName, user.FirstName);
        Assert.Equal(TestLastName, user.LastName);
        Assert.True(user.IsActive);

        var logins = await _userManager.GetLoginsAsync(user);
        Assert.Single(logins);
        Assert.Equal(TestProvider, logins[0].LoginProvider);
        Assert.Equal(TestProviderKey, logins[0].ProviderKey);
    }

    [Fact]
    public async Task ExternalLogin_DuplicateProviderKey_ReturnsBadRequest()
    {
        // Arrange
        var user = new User
        {
            UserName = TestEmail,
            Email = TestEmail,
            FirstName = TestFirstName,
            LastName = TestLastName,
            IsActive = true
        };
        await _userManager.CreateAsync(user);

        var loginInfo = new UserLoginInfo(TestProvider, TestProviderKey, TestProvider);
        await _userManager.AddLoginAsync(user, loginInfo);

        var request = new ExternalLoginRequest(
            Provider: TestProvider,
            ProviderKey: TestProviderKey,
            Email: "another@example.com",
            FirstName: "Another",
            LastName: "User");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/external-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ExternalLogin_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var user = new User
        {
            UserName = TestEmail,
            Email = TestEmail,
            FirstName = TestFirstName,
            LastName = TestLastName,
            IsActive = true
        };
        await _userManager.CreateAsync(user);

        var request = new ExternalLoginRequest(
            Provider: TestProvider,
            ProviderKey: TestProviderKey,
            Email: TestEmail,
            FirstName: TestFirstName,
            LastName: TestLastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/external-login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetExternalLogins_ReturnsProviders()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/external-logins");

        // Assert
        response.EnsureSuccessStatusCode();
        var providers = await DeserializeResponseAsync<List<string>>(response);
        Assert.NotNull(providers);
        Assert.Contains("Google", providers);
        Assert.Contains("Microsoft", providers);
    }
} 
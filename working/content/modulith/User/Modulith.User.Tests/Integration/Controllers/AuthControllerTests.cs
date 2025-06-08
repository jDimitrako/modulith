using Microsoft.AspNetCore.Mvc.Testing;
using Modulith.User.Api;
using Modulith.User.Contracts.DTOs;
using System.Net;
using System.Net.Http.Json;

namespace Modulith.User.Tests.Integration.Controllers;

public class AuthControllerTests : TestBase
{
    public AuthControllerTests(WebApplicationFactory<Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_ValidData_ShouldCreateUser()
    {
        // Arrange
        var request = new RegisterRequest(
            "test@example.com",
            "Password123!",
            "John",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await _userManager.FindByEmailAsync(request.Email);
        Assert.NotNull(user);
        Assert.Equal(request.Email, user.Email);
        Assert.Equal(request.FirstName, user.FirstName);
        Assert.Equal(request.LastName, user.LastName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnBadRequest()
    {
        // Arrange
        var email = "test@example.com";
        await CreateTestUser(email, "Password123!");

        var request = new RegisterRequest(
            email,
            "Password123!",
            "John",
            "Doe");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnTokens()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        await CreateTestUser(email, password);

        var request = new LoginRequest(email, password, false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.NotNull(result.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest(
            "test@example.com",
            "WrongPassword",
            false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_Authenticated_ShouldReturnUserInfo()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Password123!";
        await CreateTestUser(email, password);

        var token = await GetAuthToken(email, password);
        SetAuthToken(token);

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<UserInfoResponse>();
        Assert.NotNull(result);
        Assert.Equal(email, result.Email);
        Assert.Contains("User", result.Roles);
    }

    [Fact]
    public async Task GetCurrentUser_Unauthenticated_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    public override async Task DisposeAsync()
    {
        await CleanupAsync();
        await base.DisposeAsync();
    }
} 
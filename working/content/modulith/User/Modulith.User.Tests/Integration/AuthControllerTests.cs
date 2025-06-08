using Microsoft.AspNetCore.Identity;
using Modulith.User.Contracts.DTOs;
using Modulith.User.Domain.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Modulith.User.Tests.Integration;

public class AuthControllerTests : TestBase
{
    private const string TestEmail = "test@example.com";
    private const string TestPassword = "Password123!";
    private const string TestFirstName = "John";
    private const string TestLastName = "Doe";

    [Fact]
    public async Task Register_ValidData_ReturnsSuccess()
    {
        // Arrange
        var request = new RegisterRequest(
            Email: TestEmail,
            Password: TestPassword,
            FirstName: TestFirstName,
            LastName: TestLastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var user = await _userManager.FindByEmailAsync(TestEmail);
        Assert.NotNull(user);
        Assert.Equal(TestEmail, user.Email);
        Assert.Equal(TestFirstName, user.FirstName);
        Assert.Equal(TestLastName, user.LastName);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var user = new User
        {
            UserName = TestEmail,
            Email = TestEmail,
            FirstName = TestFirstName,
            LastName = TestLastName
        };
        await _userManager.CreateAsync(user, TestPassword);

        var request = new RegisterRequest(
            Email: TestEmail,
            Password: TestPassword,
            FirstName: TestFirstName,
            LastName: TestLastName);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
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
        await _userManager.CreateAsync(user, TestPassword);

        var request = new LoginRequest(
            Email: TestEmail,
            Password: TestPassword);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var loginResponse = await DeserializeResponseAsync<LoginResponse>(response);
        Assert.NotNull(loginResponse);
        Assert.NotNull(loginResponse.Token);
        Assert.NotNull(loginResponse.RefreshToken);
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsBadRequest()
    {
        // Arrange
        var request = new LoginRequest(
            Email: TestEmail,
            Password: "WrongPassword");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsNewToken()
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
        await _userManager.CreateAsync(user, TestPassword);

        var token = await GetAuthTokenAsync(TestEmail, TestPassword);
        var refreshToken = await _userManager.GenerateUserTokenAsync(
            user,
            TokenOptions.DefaultProvider,
            "RefreshToken");

        var request = new RefreshTokenRequest(
            Token: token,
            RefreshToken: refreshToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var refreshResponse = await DeserializeResponseAsync<RefreshTokenResponse>(response);
        Assert.NotNull(refreshResponse);
        Assert.NotNull(refreshResponse.Token);
        Assert.NotNull(refreshResponse.NewRefreshToken);
    }

    [Fact]
    public async Task GetCurrentUser_Authenticated_ReturnsUserInfo()
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
        await _userManager.CreateAsync(user, TestPassword);

        var token = await GetAuthTokenAsync(TestEmail, TestPassword);
        SetAuthHeader(token);

        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        response.EnsureSuccessStatusCode();
        var userInfo = await DeserializeResponseAsync<UserInfoResponse>(response);
        Assert.NotNull(userInfo);
        Assert.Equal(TestEmail, userInfo.Email);
        Assert.Equal(TestFirstName, userInfo.FirstName);
        Assert.Equal(TestLastName, userInfo.LastName);
    }

    [Fact]
    public async Task GetCurrentUser_Unauthenticated_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 
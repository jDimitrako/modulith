using Microsoft.AspNetCore.Identity;
using Modulith.User.Contracts.DTOs;
using Modulith.User.Domain.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Modulith.User.Tests.Integration;

public class UserManagementTests : TestBase
{
    private const string TestEmail = "test@example.com";
    private const string TestPassword = "Password123!";
    private const string TestFirstName = "John";
    private const string TestLastName = "Doe";

    [Fact]
    public async Task ChangePassword_ValidData_ReturnsSuccess()
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

        var request = new ChangePasswordRequest(
            CurrentPassword: TestPassword,
            NewPassword: "NewPassword123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/change-password", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var canSignIn = await _userManager.CheckPasswordAsync(user, "NewPassword123!");
        Assert.True(canSignIn);
    }

    [Fact]
    public async Task ChangePassword_InvalidCurrentPassword_ReturnsBadRequest()
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

        var request = new ChangePasswordRequest(
            CurrentPassword: "WrongPassword",
            NewPassword: "NewPassword123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/change-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsSuccess()
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

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var request = new ConfirmEmailRequest(
            Email: TestEmail,
            Token: token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/confirm-email", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var updatedUser = await _userManager.FindByEmailAsync(TestEmail);
        Assert.True(await _userManager.IsEmailConfirmedAsync(updatedUser));
    }

    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsBadRequest()
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

        var request = new ConfirmEmailRequest(
            Email: TestEmail,
            Token: "InvalidToken");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/confirm-email", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_ValidEmail_ReturnsSuccess()
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

        var request = new ForgotPasswordRequest(Email: TestEmail);

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password", request);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ResetPassword_ValidData_ReturnsSuccess()
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

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var request = new ResetPasswordRequest(
            Email: TestEmail,
            Token: token,
            NewPassword: "NewPassword123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/reset-password", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var canSignIn = await _userManager.CheckPasswordAsync(user, "NewPassword123!");
        Assert.True(canSignIn);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_ReturnsBadRequest()
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

        var request = new ResetPasswordRequest(
            Email: TestEmail,
            Token: "InvalidToken",
            NewPassword: "NewPassword123!");

        // Act
        var response = await _client.PostAsJsonAsync("/api/auth/reset-password", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
} 
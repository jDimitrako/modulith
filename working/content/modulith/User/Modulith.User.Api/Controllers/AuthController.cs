using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Modulith.User.Application.Commands.ChangePassword;
using Modulith.User.Application.Commands.ConfirmEmail;
using Modulith.User.Application.Commands.ExternalLogin;
using Modulith.User.Application.Commands.Login;
using Modulith.User.Application.Commands.RefreshToken;
using Modulith.User.Application.Commands.RegisterUser;
using Modulith.User.Application.Commands.ResetPassword;
using Modulith.User.Contracts.DTOs;
using Modulith.User.Domain.Entities;
using System.Security.Claims;

namespace Modulith.User.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthController(
        IMediator mediator,
        ILogger<AuthController> logger,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        _mediator = mediator;
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var command = new RegisterUserCommand(
                Email: request.Email,
                Password: request.Password,
                FirstName: request.FirstName,
                LastName: request.LastName);

            var userId = await _mediator.Send(command);
            return Ok(new { UserId = userId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var command = new LoginCommand(
                Email: request.Email,
                Password: request.Password,
                RememberMe: request.RememberMe);

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging in user");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var command = new RefreshTokenCommand(
                Token: request.Token,
                RefreshToken: request.RefreshToken);

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request)
    {
        try
        {
            var command = new ConfirmEmailCommand(
                UserId: request.UserId,
                Token: request.Token);

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming email");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        try
        {
            var command = new ResetPasswordCommand(Email: request.Email);
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new ChangePasswordCommand(
                UserId: userId!,
                CurrentPassword: request.CurrentPassword,
                NewPassword: request.NewPassword);

            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var response = new UserInfoResponse(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Roles: user.Roles,
                IsActive: user.IsActive,
                EmailConfirmed: await _userManager.IsEmailConfirmedAsync(user),
                ExternalLogins: (await _userManager.GetLoginsAsync(user)).Select(l => l.LoginProvider).ToList()
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting current user");
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpGet("external-login")]
    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
        return Challenge(properties, provider);
    }

    [HttpGet("external-login-callback")]
    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            return BadRequest(new { Error = "Error loading external login information" });
        }

        var result = await _signInManager.ExternalLoginSignInAsync(
            info.LoginProvider,
            info.ProviderKey,
            isPersistent: false,
            bypassTwoFactor: true);

        if (result.Succeeded)
        {
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            return Redirect(returnUrl ?? "/");
        }

        // If the user doesn't have an account, create one
        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        if (email != null)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    UserName = email,
                    Email = email,
                    FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "",
                    LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "",
                    IsActive = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (createResult.Succeeded)
                {
                    await _userManager.AddLoginAsync(user, info);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Redirect(returnUrl ?? "/");
                }
            }
            else
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Redirect(returnUrl ?? "/");
            }
        }

        return BadRequest(new { Error = "Error during external login" });
    }
} 
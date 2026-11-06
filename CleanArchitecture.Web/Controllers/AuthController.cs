using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Features.Auth.Commands.GoogleLogin;
using CleanArchitecture.Application.Features.Auth.Commands.Login;
using CleanArchitecture.Application.Features.Auth.Commands.Register;
using CleanArchitecture.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArchitecture.Web.Controllers;

public class AuthController : Controller
{
    private readonly IMediator _mediator;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;
    private const string ImpersonationStackClaim = "ImpersonationUserId";

    public AuthController(
        IMediator mediator,
        SignInManager<ApplicationUser> signInManager,
        IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory)
    {
        _mediator = mediator;
        _signInManager = signInManager;
        _claimsPrincipalFactory = claimsPrincipalFactory;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        try
        {
            var user = await _mediator.Send(command);

            await SignInUserAsync(user.UserId, command.RememberMe);

            //return RedirectToAction("Index", "Home");

            // Determine the user's actual default role
            var roles = await _signInManager.UserManager.GetRolesAsync(
                await _signInManager.UserManager.FindByIdAsync(user.UserId)
                ?? throw new InvalidOperationException("User not found."));

            var activeRole = roles.Contains(RoleConstants.SuperAdmin)
                ? RoleConstants.SuperAdmin
                : roles.FirstOrDefault();

            if (activeRole == null)
                return RedirectToAction(nameof(AccessDenied));

            return RedirectToDashboard(activeRole);
        }
        catch (UnauthorizedAccessException)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");

            return View(command);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        if (!ModelState.IsValid)
            return View(command);

        try
        {
            var user = await _mediator.Send(command);

            await SignInUserAsync(user.UserId);

            //return RedirectToAction("Index", "Home");


            await SignInUserAsync(user.UserId);

            var registeredUser = await _signInManager.UserManager
                .FindByIdAsync(user.UserId);

            if (registeredUser == null)
                return RedirectToAction(nameof(AccessDenied));

            var roles = await _signInManager.UserManager
                .GetRolesAsync(registeredUser);

            var activeRole = roles.FirstOrDefault();

            return activeRole == null
                ? RedirectToAction(nameof(AccessDenied))
                : RedirectToDashboard(activeRole);

        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                string.Empty,
                ex.Message);

            return View(command);
        }
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GoogleLogin(
        GoogleLoginCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid Google credential.");

        try
        {
            var user = await _mediator.Send(command);

            await SignInUserAsync(user.UserId);

            return RedirectToAction("Index", "Home");
        }
        catch (UnauthorizedAccessException)
        {
            TempData["Error"] = "Google authentication failed.";

            return RedirectToAction(nameof(Login));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var stack = User.FindAll(ImpersonationStackClaim)
                .Select(c => c.Value)
                .ToList();

        if (stack.Count > 0)
        {
            // Restore the previous user instead of logging out
            return await StopImpersonation();
        }

        await _signInManager.SignOutAsync();

        return RedirectToAction(nameof(Login));
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> SwitchRole(string role)
    {
        // Get original authenticated user
        var user = await _signInManager.UserManager.GetUserAsync(User);

        if (user == null)
            return Unauthorized();

        // Verify original database role
        var originalRoles = await _signInManager.UserManager.GetRolesAsync(user);

        if (!originalRoles.Contains(RoleConstants.SuperAdmin))
            return Forbid();

        // Validate requested role
        var validRoles = new[]
        {
                RoleConstants.SuperAdmin,
                RoleConstants.Admin,
                RoleConstants.Manager,
                RoleConstants.Employee,
                RoleConstants.Customer
            };

        var selectedRole = validRoles.FirstOrDefault(
            x => x.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (selectedRole == null)
            return BadRequest("Invalid role.");

        // Create fresh authentication principal
        var principal = await _claimsPrincipalFactory.CreateAsync(user);

        var identity = principal.Identities
            .FirstOrDefault(x => x.IsAuthenticated);

        if (identity == null)
            return Unauthorized();

        // Remove existing role claims
        foreach (var claim in identity.FindAll(identity.RoleClaimType).ToList())
        {
            identity.RemoveClaim(claim);
        }

        // Add selected active role
        identity.AddClaim(
            new Claim(identity.RoleClaimType, selectedRole));

        // Remove previous ActiveRole claim
        foreach (var claim in identity.FindAll("ActiveRole").ToList())
        {
            identity.RemoveClaim(claim);
        }

        // Add new ActiveRole claim
        identity.AddClaim(
            new Claim("ActiveRole", selectedRole));

        // Preserve authentication properties
        var authResult = await HttpContext.AuthenticateAsync(
            IdentityConstants.ApplicationScheme);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            authResult.Properties ?? new AuthenticationProperties());

        TempData["Success"] =
            $"Successfully switched to {selectedRole} mode.";

        return RedirectToAction("Index", "Home");
    }



    private async Task<IActionResult> ImpersonateRoleAsync(string role)
    {
        // Get current authenticated user
        var currentUserId = _signInManager.UserManager.GetUserId(User);

        if (string.IsNullOrEmpty(currentUserId))
            return Unauthorized();

        // Read previous impersonation history
        var stack = User.FindAll(ImpersonationStackClaim)
            .Select(c => c.Value)
            .ToList();

        // Save current user if this is the first impersonation
        if (stack.Count == 0)
        {
            stack.Add(currentUserId);
        }
        else if (stack.Contains(currentUserId))
        {
            return Forbid();
        }
        else
        {
            stack.Add(currentUserId);
        }

        // Find target account
        var users = await _signInManager.UserManager.GetUsersInRoleAsync(role);

        var targetUser = users.FirstOrDefault();

        if (targetUser == null)
        {
            TempData["Error"] = $"{role} account not found.";
            return RedirectToAction("Index", "Home");
        }

        if (targetUser.Id == currentUserId)
            return Forbid();

        // Create target user's authentication principal
        var principal = await _signInManager.CreateUserPrincipalAsync(
            targetUser);

        var identity = principal.Identities
            .FirstOrDefault(x => x.IsAuthenticated);

        if (identity == null)
            return Unauthorized();

        // Store impersonation history in protected auth cookie
        foreach (var userId in stack)
        {
            identity.AddClaim(new Claim(ImpersonationStackClaim, userId));
        }

        // Preserve authentication properties
        var authResult = await HttpContext.AuthenticateAsync(
            IdentityConstants.ApplicationScheme);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            authResult.Properties ?? new AuthenticationProperties());

        TempData["Success"] =
            $"Successfully logged in as {role}.";

        //return RedirectToAction("Index", "Home");
        return RedirectToDashboard(role);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> StopImpersonation()
    {
        // Read impersonation history
        var stack = User.FindAll(ImpersonationStackClaim)
            .Select(c => c.Value)
            .ToList();

        if (stack.Count == 0)
        {
            TempData["Error"] = "No impersonation session found.";
            return RedirectToAction("Index", "Home");
        }

        // Get immediately previous user
        var previousUserId = stack.Last();

        var previousUser = await _signInManager.UserManager.FindByIdAsync(
            previousUserId);

        if (previousUser == null)
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        // Create previous user's authentication principal
        var principal = await _signInManager.CreateUserPrincipalAsync(
            previousUser);

        var identity = principal.Identities
            .FirstOrDefault(x => x.IsAuthenticated);

        if (identity == null)
            return Unauthorized();

        // Remove the restored user from the history
        stack.RemoveAt(stack.Count - 1);

        // Preserve remaining impersonation history
        foreach (var userId in stack)
        {
            identity.AddClaim(
                new Claim(ImpersonationStackClaim, userId));
        }

        // Preserve authentication properties
        var authResult = await HttpContext.AuthenticateAsync(
            IdentityConstants.ApplicationScheme);

        await HttpContext.SignInAsync(
            IdentityConstants.ApplicationScheme,
            principal,
            authResult.Properties ?? new AuthenticationProperties());

        TempData["Success"] =
            $"Returned to {previousUser.UserName}.";

        //return RedirectToAction("Index", "Home");


        var previousRoles = await _signInManager.UserManager.GetRolesAsync(previousUser);

        var previousRole = previousRoles.Contains(RoleConstants.SuperAdmin)
            ? RoleConstants.SuperAdmin
            : previousRoles.FirstOrDefault();

        return previousRole == null
            ? RedirectToAction(nameof(AccessDenied))
            : RedirectToDashboard(previousRole);

    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.SuperAdmin + "," + RoleConstants.SystemAdmin)]
    public Task<IActionResult> ImpersonateAdmin()
    {
        return ImpersonateRoleAsync(RoleConstants.Admin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.Admin)]
    public Task<IActionResult> ImpersonateManager()
    {
        return ImpersonateRoleAsync(RoleConstants.Manager);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Manager)]
    public Task<IActionResult> ImpersonateEmployee()
    {
        return ImpersonateRoleAsync(RoleConstants.Employee);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleConstants.Admin + "," + RoleConstants.Manager)]
    public Task<IActionResult> ImpersonateCustomer()
    {
        return ImpersonateRoleAsync(RoleConstants.Customer);
    }



    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInUserAsync(string userId, bool rememberMe = false)
    {
        var user = await _signInManager.UserManager.FindByIdAsync(userId);

        if (user is null)
        {
            throw new InvalidOperationException("User not found.");
        }

        await _signInManager.SignInAsync(
            user,
            isPersistent: rememberMe);
    }

    private IActionResult RedirectToDashboard(string activeRole)
    {
        return activeRole switch
        {
            RoleConstants.SuperAdmin =>
                RedirectToAction("Index", "SuperAdmin"),

            RoleConstants.SystemAdmin =>
                RedirectToAction("Index", "SystemAdmin"),

            RoleConstants.Admin =>
                RedirectToAction("Index", "Admin"),

            RoleConstants.Manager =>
                RedirectToAction("Index", "Manager"),

            RoleConstants.Employee =>
                RedirectToAction("Index", "Employee"),

            RoleConstants.Customer =>
                RedirectToAction("Index", "Customer"),

            _ => RedirectToAction("AccessDenied", "Auth")
        };
    }

    private async Task<string?> GetActiveRoleAsync()
    {
        var user = await _signInManager.UserManager.GetUserAsync(User);

        if (user == null)
            return null;

        // Use the active role claim when available
        var activeRole = User.FindFirst("ActiveRole")?.Value;

        if (!string.IsNullOrWhiteSpace(activeRole))
            return activeRole;

        // Otherwise use the user's actual database role
        var roles = await _signInManager.UserManager.GetRolesAsync(user);

        return roles.FirstOrDefault();
    }

}

using CleanArchitecture.Infrastructure.Authorization;

namespace CleanArchitecture.Web.Middleware;

public class PermissionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<PermissionMiddleware> _logger;

    public PermissionMiddleware(RequestDelegate next, ILogger<PermissionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IPermissionService permissionService)
    {
        _logger.LogInformation(
            "PermissionMiddleware executing: {Path}",
            context.Request.Path);

        if (context.User.Identity?.IsAuthenticated == true)
        {
            var permissions = await permissionService.GetUserPermissionsAsync(context.User);

            context.Items["UserPermissions"] = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);

            //var allClaims =
            //    await permissionService.GetAllClaimsAsync(context.User);

            //context.Items["UserClaims"] = allClaims;

            //// Maintain compatibility with existing code
            //context.Items["UserPermissions"] =
            //    allClaims.TryGetValue(
            //        ClaimConstants.Permission,
            //        out var permissions)
            //        ? permissions.ToHashSet(
            //            StringComparer.OrdinalIgnoreCase)
            //        : new HashSet<string>(
            //            StringComparer.OrdinalIgnoreCase);
        }

        await _next(context);
    }
}
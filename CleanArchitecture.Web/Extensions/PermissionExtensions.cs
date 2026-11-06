using System.Security.Claims;

namespace CleanArchitecture.Web.Extensions
{
    public static class PermissionExtensions
    {
        // 1. Check a single permission
        public static bool HasPermission(this ClaimsPrincipal user, IEnumerable<string> permissions, string permission)
        {
            return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }


        // 2. Check multiple permissions (ANY)
        public static bool HasAnyPermission(this ClaimsPrincipal user, IEnumerable<string> permissions, params string[] requiredPermissions)
        {
            return permissions != null && requiredPermissions.Any(p => permissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        }


        // 3. Check multiple permissions (ALL)
        public static bool HasAllPermissions(this ClaimsPrincipal user, IEnumerable<string> permissions, params string[] requiredPermissions)
        {
            return permissions != null && requiredPermissions.All(p => permissions.Contains(p, StringComparer.OrdinalIgnoreCase));
        }

        // 4. Get permissions from HttpContext
        public static HashSet<string> GetPermissions(this HttpContext context)
        {
            return context.Items["UserPermissions"] as HashSet<string> ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        // 5. Check permission directly from HttpContext
        public static bool HasPermission(this HttpContext context, string permission)
        {
            return context.GetPermissions().Contains(permission);
        }

        // 6. Check permission directly from ClaimsPrincipal + HttpContext
        public static bool HasPermission(this ClaimsPrincipal user, HttpContext context, string permission)
        {
            return context.HasPermission(permission);
        }

        // 7. Check if user has any permission
        public static bool HasAnyPermission(this HttpContext context, params string[] permissions)
        {
            var userPermissions = context.GetPermissions();
            return permissions.Any(p => userPermissions.Contains(p));
        }

        // 8. Check if user has all permissions
        public static bool HasAllPermissions(this HttpContext context, params string[] permissions)
        {
            var userPermissions = context.GetPermissions();
            return permissions.All(p => userPermissions.Contains(p));
        }
    }
}
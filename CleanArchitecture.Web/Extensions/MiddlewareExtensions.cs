using CleanArchitecture.Web.Middleware;

namespace CleanArchitecture.Web.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<PermissionMiddleware>();
        }
    }
}

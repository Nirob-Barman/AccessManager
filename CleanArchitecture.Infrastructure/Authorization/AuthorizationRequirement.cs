using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.Infrastructure.Authorization
{
    public class AuthorizationRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }

        public AuthorizationRequirement(string permission)
        {
            Permission = permission;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionAuthorizationHandler(
            IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthorizationRequirement requirement)
        {
            //if (context.User.HasClaim(PermissionConstants.ClaimType, requirement.Permission))
            //{
            //    context.Succeed(requirement);
            //}

            //return Task.CompletedTask;

            var hasPermission =
                await _permissionService.HasPermissionAsync(
                    context.User,
                    "Permission",
                    requirement.Permission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
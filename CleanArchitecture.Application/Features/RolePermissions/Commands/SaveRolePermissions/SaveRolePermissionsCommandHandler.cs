using MediatR;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RolePermissions.Commands.SaveRolePermissions;

public class SaveRolePermissionsCommandHandler
    : IRequestHandler<SaveRolePermissionsCommand, bool>
{
    private readonly IRolePermissionService _rolePermissionService;

    public SaveRolePermissionsCommandHandler(
        IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    public async Task<bool> Handle(
        SaveRolePermissionsCommand request,
        CancellationToken cancellationToken)
    {
        return await _rolePermissionService.SaveRolePermissionsAsync(
            request.RoleId,
            request.Permissions,
            cancellationToken);
    }
}
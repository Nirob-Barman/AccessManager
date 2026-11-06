using MediatR;
using CleanArchitecture.Application.Features.RolePermissions.Dtos;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RolePermissions.Queries.GetRolePermissions;

public class GetRolePermissionsQueryHandler
    : IRequestHandler<GetRolePermissionsQuery, RolePermissionDto>
{
    private readonly IRolePermissionService _rolePermissionService;

    public GetRolePermissionsQueryHandler(
        IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    public async Task<RolePermissionDto> Handle(
        GetRolePermissionsQuery request,
        CancellationToken cancellationToken)
    {
        return await _rolePermissionService.GetRolePermissionsAsync(
            request.RoleId,
            cancellationToken);
    }
}
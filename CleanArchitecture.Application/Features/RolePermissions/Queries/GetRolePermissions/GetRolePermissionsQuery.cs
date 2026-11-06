using MediatR;
using CleanArchitecture.Application.Features.RolePermissions.Dtos;

namespace CleanArchitecture.Application.Features.RolePermissions.Queries.GetRolePermissions;

public record GetRolePermissionsQuery(string? RoleId)
    : IRequest<RolePermissionDto>;
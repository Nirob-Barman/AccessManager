using MediatR;

namespace CleanArchitecture.Application.Features.RolePermissions.Commands.SaveRolePermissions;

public record SaveRolePermissionsCommand(
    string RoleId,
    List<string> Permissions
) : IRequest<bool>;
using MediatR;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.UpdateRole;

public record UpdateRoleCommand(string Id, string Name)
    : IRequest<(bool Succeeded, string[] Errors)>;
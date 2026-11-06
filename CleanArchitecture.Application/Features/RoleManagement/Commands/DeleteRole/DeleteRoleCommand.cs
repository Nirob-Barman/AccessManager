using MediatR;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.DeleteRole;

public record DeleteRoleCommand(string Id)
    : IRequest<(bool Succeeded, string Error)>;
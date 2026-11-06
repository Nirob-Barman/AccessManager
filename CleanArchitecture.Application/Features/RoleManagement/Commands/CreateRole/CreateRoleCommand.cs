using MediatR;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.CreateRole;

public record CreateRoleCommand(string Name)
    : IRequest<(bool Succeeded, string[] Errors)>;
using MediatR;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.AssignUsers;

public record AssignUsersCommand(
    string RoleId,
    List<string> SelectedUserIds)
    : IRequest<(bool Succeeded, string Error)>;
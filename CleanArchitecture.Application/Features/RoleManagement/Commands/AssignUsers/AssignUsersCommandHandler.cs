using MediatR;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.AssignUsers;

public class AssignUsersCommandHandler
    : IRequestHandler<AssignUsersCommand, (bool Succeeded, string Error)>
{
    private readonly IRoleManagementService _service;

    public AssignUsersCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<(bool Succeeded, string Error)> Handle(
        AssignUsersCommand request,
        CancellationToken cancellationToken)
    {
        return await _service.AssignUsersAsync(request.RoleId, request.SelectedUserIds);
    }
}
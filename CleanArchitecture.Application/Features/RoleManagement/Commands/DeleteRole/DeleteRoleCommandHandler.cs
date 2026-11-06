using MediatR;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Application.Features.RoleManagement.Commands.DeleteRole;

public class deleteRoleCommandHandler
    : IRequestHandler<DeleteRoleCommand, (bool Succeeded, string Error)>
{
    private readonly IRoleManagementService _service;

    public deleteRoleCommandHandler(IRoleManagementService service)
    {
        _service = service;
    }

    public async Task<(bool Succeeded, string Error)> Handle(
        DeleteRoleCommand request,
        CancellationToken cancellationToken)
    {
        return await _service.DeleteRoleAsync(request.Id);
    }
}
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoles;
using CleanArchitecture.Application.Features.RoleManagement.Queries.GetRoleById;
using CleanArchitecture.Application.Features.RoleManagement.Queries.GetUsersForRole;
using CleanArchitecture.Application.Features.RoleManagement.Commands.CreateRole;
using CleanArchitecture.Application.Features.RoleManagement.Commands.UpdateRole;
using CleanArchitecture.Application.Features.RoleManagement.Commands.DeleteRole;
using CleanArchitecture.Application.Features.RoleManagement.Commands.AssignUsers;
using CleanArchitecture.Web.Models.RoleManagement;

namespace CleanArchitecture.Web.Controllers;

[Authorize(Roles = RoleConstants.SuperAdmin + "," + RoleConstants.Admin)]
public class RoleManagementController : Controller
{
    private readonly IMediator _mediator;

    public RoleManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: Index
    public async Task<IActionResult> Index()
    {
        var roles = await _mediator.Send(new GetRolesQuery());

        var model = roles.Select(r => new RoleManagementViewModel
        {
            Id = r.Id,
            Name = r.Name,
            UserCount = r.UserCount
        }).ToList();

        return View(model);
    }

    // GET: Create
    public IActionResult Create()
    {
        return View(new CreateRoleViewModel());
    }

    // POST: Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRoleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _mediator.Send(
            new CreateRoleCommand(model.Name));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(nameof(model.Name), error);

            return View(model);
        }

        TempData["Success"] = "Role created successfully.";

        return RedirectToAction(nameof(Index));
    }

    // GET: Edit
    public async Task<IActionResult> Edit(string id)
    {
        var role = await _mediator.Send(new GetRoleByIdQuery(id));

        if (role == null)
            return NotFound();

        var model = new EditRoleViewModel
        {
            Id = role.Id,
            Name = role.Name
        };

        return View(model);
    }

    // POST: Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditRoleViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _mediator.Send(
            new UpdateRoleCommand(model.Id, model.Name));

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError(nameof(model.Name), error);

            return View(model);
        }

        TempData["Success"] = "Role updated successfully.";

        return RedirectToAction(nameof(Index));
    }

    // POST: Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));

        if (!result.Succeeded)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Role deleted successfully.";

        return RedirectToAction(nameof(Index));
    }

    // GET: AssignUsers
    public async Task<IActionResult> AssignUsers(string roleId)
    {
        var result = await _mediator.Send(
            new GetUsersForRoleQuery(roleId));

        if (result == null)
            return NotFound();

        var model = new AssignUsersViewModel
        {
            RoleId = result.RoleId,
            RoleName = result.RoleName,

            Users = result.Users.Select(u => new UserRoleItemViewModel
            {
                UserId = u.UserId,
                UserName = u.UserName,
                Email = u.Email,
                IsSelected = u.IsSelected
            }).ToList()
        };

        return View(model);
    }

    // POST: AssignUsers
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignUsers(
        AssignUsersViewModel model)
    {
        var selectedUserIds = (model.Users ?? new())
            .Where(u => u.IsSelected)
            .Select(u => u.UserId)
            .ToList();

        var result = await _mediator.Send(
            new AssignUsersCommand(model.RoleId, selectedUserIds));

        if (!result.Succeeded)
        {
            TempData["Error"] = result.Error;
        }
        else
        {
            TempData["Success"] =
                "User role assignments updated successfully.";
        }

        return RedirectToAction(
            nameof(AssignUsers),
            new { roleId = model.RoleId });
    }
}
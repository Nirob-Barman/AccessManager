using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Application.Features.RolePermissions.Queries.GetRolePermissions;
using CleanArchitecture.Application.Features.RolePermissions.Commands.SaveRolePermissions;
using CleanArchitecture.Web.Models.RolePermissions;
using CleanArchitecture.Application.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CleanArchitecture.Web.Controllers;

[Authorize(Roles = RoleConstants.SuperAdmin + "," + RoleConstants.Admin)]
public class RolePermissionController : Controller
{
    private readonly IMediator _mediator;

    public RolePermissionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? roleId)
    {
        var result = await _mediator.Send(
            new GetRolePermissionsQuery(roleId));

        var model = new RolePermissionViewModel
        {
            RoleId = result.RoleId,

            Roles = result.Roles.Select(r => new SelectListItem
            {
                Value = r.Id,
                Text = r.Name,
                Selected = r.Id == result.RoleId
            }).ToList(),

            Menus = result.Menus,

            AssignedPermissions = result.AssignedPermissions
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SavePermissions(
        string roleId,
        List<string> permissions)
    {
        var success = await _mediator.Send(
            new SaveRolePermissionsCommand(
                roleId,
                permissions ?? new List<string>()));

        if (!success)
        {
            TempData["Error"] =
                "Failed to save permissions. Please check the role and selected permissions.";
        }
        else
        {
            TempData["Success"] =
                "Permissions updated successfully.";
        }

        return RedirectToAction(nameof(Index), new { roleId });
    }
}
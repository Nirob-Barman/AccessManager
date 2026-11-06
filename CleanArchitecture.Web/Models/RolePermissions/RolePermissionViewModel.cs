using Microsoft.AspNetCore.Mvc.Rendering;
using CleanArchitecture.Application.Common.Models.Authorization;

namespace CleanArchitecture.Web.Models.RolePermissions;

public class RolePermissionViewModel
{
    public string RoleId { get; set; } = string.Empty;

    public List<SelectListItem> Roles { get; set; } = new();

    public List<MenuItem> Menus { get; set; } = new();

    public List<string> AssignedPermissions { get; set; } = new();
}
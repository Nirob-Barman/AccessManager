using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Web.Models.ManagerDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Controllers;

[Authorize(Roles = RoleConstants.Manager)]
public class ManagerController : Controller
{
    private readonly IIdentityService _identityService;

    public ManagerController(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public IActionResult Index()
    {
        var model = new ManagerDashboardViewModel();
        return View(model);
    }
}
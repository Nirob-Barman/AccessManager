using CleanArchitecture.Application.Constants;
using CleanArchitecture.Web.Models.SuperAdminDashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Controllers;

[Authorize(Roles = RoleConstants.SuperAdmin)]
public class SuperAdminController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new SuperAdminDashboardViewModel();

        return View(model);
    }
}
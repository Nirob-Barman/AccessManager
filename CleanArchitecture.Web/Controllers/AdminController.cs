using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CleanArchitecture.Application.Constants;
using CleanArchitecture.Web.Models.AdminDashboard;

namespace CleanArchitecture.Web.Controllers;

[Authorize(Roles = RoleConstants.Admin)]
public class AdminController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        var model = new AdminDashboardViewModel
        {
            TotalUsers = 0,
            ActiveUsers = 0,
            TotalManagers = 0,
            TotalEmployees = 0,
            TotalCustomers = 0
        };

        return View(model);
    }
}
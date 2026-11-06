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

    public async Task<IActionResult> Index()
    {
        //var employees = await _identityService.GetUsersInRoleAsync(
        //    RoleConstants.Employee);

        //var customers = await _identityService.GetCustomerCountAsync();

        //var model = new ManagerDashboardViewModel
        //{
        //    TotalEmployees = employees.Count,
        //    ActiveEmployees = employees.Count(x => x.IsActive),
        //    InactiveEmployees = employees.Count(x => !x.IsActive),
        //    TotalCustomers = customers,
        //    TotalUsers = employees.Count + customers,

        //    RecentEmployees = employees
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Take(5)
        //        .Select(x => new ManagerEmployeeDto
        //        {
        //            FullName = x.FullName,
        //            Email = x.Email,
        //            Role = RoleConstants.Employee,
        //            CreatedAt = x.CreatedAt,
        //            IsActive = x.IsActive
        //        })
        //        .ToList()
        //};

        var model = new ManagerDashboardViewModel();
        return View(model);
    }
}
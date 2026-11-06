using CleanArchitecture.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CleanArchitecture.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Employee)]
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();

            var model = new EmployeeDashboardViewModel
            {
                EmployeeId = User.FindFirstValue("EmployeeId")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? "N/A",

                FullName = User.FindFirstValue(ClaimTypes.Name)
                    ?? User.FindFirstValue("FullName")
                    ?? "Employee",

                Email = User.FindFirstValue(ClaimTypes.Email)
                    ?? "Not available",

                PhoneNumber = User.FindFirstValue(ClaimTypes.MobilePhone)
                    ?? User.FindFirstValue(ClaimTypes.HomePhone)
                    ?? "Not available",

                Role = User.FindFirstValue(ClaimTypes.Role)
                    ?? "Employee",

                IsAuthenticated = User.Identity?.IsAuthenticated == true,

                Permissions = claims
                    .Where(c =>
                        c.Type.Equals("permission",
                            StringComparison.OrdinalIgnoreCase)
                        || c.Type.EndsWith("/permission",
                            StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Value)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList()
            };

            return View(model);
        }
    }

    public class EmployeeDashboardViewModel
    {
        public string EmployeeId { get; set; } = "N/A";

        public string FullName { get; set; } = "Employee";

        public string Email { get; set; } = "Not available";

        public string PhoneNumber { get; set; } = "Not available";

        public string Role { get; set; } = "Employee";

        public bool IsAuthenticated { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}
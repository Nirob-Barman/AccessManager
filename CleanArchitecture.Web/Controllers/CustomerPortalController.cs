using System.Security.Claims;
using CleanArchitecture.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Customer)]
    public class CustomerPortalController : Controller
    {
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();

            var model = new CustomerDashboardViewModel
            {
                CustomerId =
                    User.FindFirstValue("CustomerId")
                    ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? "N/A",

                FullName =
                    User.FindFirstValue("FullName")
                    ?? User.FindFirstValue(ClaimTypes.Name)
                    ?? "Customer",

                Email =
                    User.FindFirstValue(ClaimTypes.Email)
                    ?? "Not available",

                PhoneNumber =
                    User.FindFirstValue(ClaimTypes.MobilePhone)
                    ?? User.FindFirstValue(ClaimTypes.HomePhone)
                    ?? "Not available",

                Role =
                    User.FindFirstValue(ClaimTypes.Role)
                    ?? "Customer",

                IsAuthenticated =
                    User.Identity?.IsAuthenticated == true,

                AccountStatus =
                    User.FindFirstValue("AccountStatus")
                    ?? "Not available",

                Permissions = claims
                    .Where(c =>
                        c.Type.Equals(
                            "permission",
                            StringComparison.OrdinalIgnoreCase)
                        || c.Type.EndsWith(
                            "/permission",
                            StringComparison.OrdinalIgnoreCase))
                    .Select(c => c.Value)
                    .Distinct()
                    .OrderBy(p => p)
                    .ToList()
            };

            return View(model);
        }
    }

    public class CustomerDashboardViewModel
    {
        public string CustomerId { get; set; } = "N/A";

        public string FullName { get; set; } = "Customer";

        public string Email { get; set; } = "Not available";

        public string PhoneNumber { get; set; } = "Not available";

        public string Role { get; set; } = "Customer";

        public string AccountStatus { get; set; } = "Not available";

        public bool IsAuthenticated { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}
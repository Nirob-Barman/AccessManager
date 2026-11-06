using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace PermissionControl.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        // Temporary in-memory dummy data
        private static readonly List<UserDummy> Users = new()
        {
            new UserDummy
            {
                Id = 1,
                Name = "John Smith",
                Email = "john@example.com",
                IsActive = true
            },
            new UserDummy
            {
                Id = 2,
                Name = "Sarah Johnson",
                Email = "sarah@example.com",
                IsActive = true
            },
            new UserDummy
            {
                Id = 3,
                Name = "Michael Brown",
                Email = "michael@example.com",
                IsActive = false
            },
            new UserDummy
            {
                Id = 4,
                Name = "Emily Davis",
                Email = "emily@example.com",
                IsActive = true
            },
            new UserDummy
            {
                Id = 5,
                Name = "David Wilson",
                Email = "david@example.com",
                IsActive = false
            }
        };

        [Authorize(Policy = "Users.List.View")]
        public IActionResult Index()
        {
            return View(Users.OrderBy(x => x.Id).ToList());
        }

        [Authorize(Policy = "Users.List.Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.Create")]
        public IActionResult Create(string name, string email)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Name and email are required.";
                return RedirectToAction(nameof(Create));
            }

            if (Users.Any(x => x.Email.Equals(
                email.Trim(),
                StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Error"] = "Email already exists.";
                return RedirectToAction(nameof(Create));
            }

            int newId = Users.Any()
                ? Users.Max(x => x.Id) + 1
                : 1;

            Users.Add(new UserDummy
            {
                Id = newId,
                Name = name.Trim(),
                Email = email.Trim(),
                IsActive = true
            });

            TempData["Success"] = "User created successfully.";

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Policy = "Users.List.Create")]
        public IActionResult Edit(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.Create")]
        public IActionResult Edit(int id, string name, string email)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email))
            {
                TempData["Error"] = "Name and email are required.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            if (Users.Any(x => x.Id != id &&
                x.Email.Equals(email.Trim(),
                    StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Error"] = "Email already exists.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            user.Name = name.Trim();
            user.Email = email.Trim();

            TempData["Success"] = "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.Delete")]
        public IActionResult Delete(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            Users.Remove(user);

            TempData["Success"] = "User deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.ResetPassword")]
        public IActionResult ResetPassword(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            TempData["Success"] =
                $"Password reset requested for {user.Name}.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.Activate")]
        public IActionResult Activate(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            user.IsActive = true;

            TempData["Success"] = "User activated successfully.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Users.List.Deactivate")]
        public IActionResult Deactivate(int id)
        {
            var user = Users.FirstOrDefault(x => x.Id == id);

            if (user == null)
                return NotFound();

            user.IsActive = false;

            TempData["Success"] = "User deactivated successfully.";

            return RedirectToAction(nameof(Index));
        }


        [Authorize(Policy = "Users.List.Export")]
        public IActionResult Export()
        {
            var csv = new StringBuilder();

            csv.AppendLine("Id,Name,Email,Status");

            foreach (var user in Users)
            {
                csv.AppendLine(
                    $"{user.Id},\"{user.Name.Replace("\"", "\"\"")}\"," +
                    $"\"{user.Email.Replace("\"", "\"\"")}\"," +
                    $"{(user.IsActive ? "Active" : "Inactive")}");
            }

            return File(
                Encoding.UTF8.GetBytes(csv.ToString()),
                "text/csv",
                "Users.csv");
        }
    }

    public class UserDummy
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public bool IsActive { get; set; }
    }
}

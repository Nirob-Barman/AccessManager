using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using CleanArchitecture.Infrastructure.Identity;

namespace CleanArchitecture.Infrastructure.Persistence.Seed;

public static class IdentitySeeder
{
    public static async Task Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        // Step 1: Create roles
        await CreateRolesAsync(roleManager);

        // Step 2: Create default users
        await CreateDefaultUsersAsync(userManager);

        // Step 3: Assign roles to users
        await AssignUserRolesAsync(userManager);
    }



    // 1. Create Roles
    private static async Task CreateRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
                "SuperAdmin",
                "SystemAdmin",
                "Admin",
                "Manager",
                "Employee",
                "Customer"
            };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(
                    new IdentityRole(role));

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }
    }


    // 2. Create Default Users
    private static async Task CreateDefaultUsersAsync(
        UserManager<ApplicationUser> userManager)
    {
        var defaultUsers = new[]
        {
                new
                {
                    Email = "superadmin@example.com",
                    FullName = "Default Super Admin",
                    Password = "SuperAdmin@123"
                },
                new
                {
                    Email = "systemadmin@example.com",
                    FullName = "Default System Admin",
                    Password = "SystemAdmin@123"
                },
                new
                {
                    Email = "admin@example.com",
                    FullName = "Default Admin",
                    Password = "Admin@123"
                },
                new
                {
                    Email = "manager@example.com",
                    FullName = "Default Manager",
                    Password = "Manager@123"
                },
                new
                {
                    Email = "employee@example.com",
                    FullName = "Default Employee",
                    Password = "Employee@123"
                },
                new
                {
                    Email = "employee2@example.com",
                    FullName = "David Wilson",
                    Password = "Employee2@123"
                },
                new
                {
                    Email = "employee3@example.com",
                    FullName = "James Taylor",
                    Password = "Employee3@123"
                },
                new
                {
                    Email = "employee4@example.com",
                    FullName = "Robert Johnson",
                    Password = "Employee4@123"
                },
                new
                {
                    Email = "employee5@example.com",
                    FullName = "Daniel Martin",
                    Password = "Employee5@123"
                },
                new
                {
                    Email = "customer@example.com",
                    FullName = "Default Customer",
                    Password = "Customer@123"
                },
                new
                {
                    Email = "customer2@example.com",
                    FullName = "Olivia Brown",
                    Password = "Customer2@123"
                },
                new
                {
                    Email = "customer3@example.com",
                    FullName = "Sophia Davis",
                    Password = "Customer3@123"
                },
                new
                {
                    Email = "customer4@example.com",
                    FullName = "Isabella Miller",
                    Password = "Customer4@123"
                },
                new
                {
                    Email = "customer5@example.com",
                    FullName = "Mia Wilson",
                    Password = "Customer5@123"
                }
            };

        foreach (var userData in defaultUsers)
        {
            await CreateUserAsync(
                userManager,
                userData.Email,
                userData.FullName,
                userData.Password
            );
        }
    }


    // 3. Create Individual User
    private static async Task CreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string fullName,
        string password)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user != null)
            return;

        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FullName = fullName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            throw new Exception(
                string.Join(", ",
                    result.Errors.Select(e => e.Description)));
        }
    }


    // 4. Assign Roles to Users
    private static async Task AssignUserRolesAsync(
        UserManager<ApplicationUser> userManager)
    {
        var userRoles = new[]
        {
                new
                {
                    Email = "superadmin@example.com",
                    Role = "SuperAdmin"
                },
                new
                {
                    Email = "systemadmin@example.com",
                    Role = "SystemAdmin"
                },
                new
                {
                    Email = "admin@example.com",
                    Role = "Admin"
                },
                new
                {
                    Email = "manager@example.com",
                    Role = "Manager"
                },
                new
                {
                    Email = "employee@example.com",
                    Role = "Employee"
                },
                new
                {
                    Email = "employee2@example.com",
                    Role = "Employee"
                },
                new
                {
                    Email = "employee3@example.com",
                    Role = "Employee"
                },
                new
                {
                    Email = "employee4@example.com",
                    Role = "Employee"
                },
                new
                {
                    Email = "employee5@example.com",
                    Role = "Employee"
                },
                new
                {
                    Email = "customer@example.com",
                    Role = "Customer"
                },
                new
                {
                    Email = "customer2@example.com",
                    Role = "Customer"
                },
                new
                {
                    Email = "customer3@example.com",
                    Role = "Customer"
                },
                new
                {
                    Email = "customer4@example.com",
                    Role = "Customer"
                },
                new
                {
                    Email = "customer5@example.com",
                    Role = "Customer"
                }
            };

        foreach (var item in userRoles)
        {
            var user = await userManager.FindByEmailAsync(item.Email);

            if (user == null)
                continue;

            if (!await userManager.IsInRoleAsync(user, item.Role))
            {
                var result = await userManager.AddToRoleAsync(
                    user, item.Role);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }
        }
    }


}

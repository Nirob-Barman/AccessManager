using CleanArchitecture.Application;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Authorization;
using CleanArchitecture.Infrastructure.Persistence.Seed;
using CleanArchitecture.Web.Extensions;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{
    foreach (var menu in MenuConfiguration.GetMenus())    
    {
        foreach (var submenu in menu.SubMenus)
        {
            foreach (var permission in submenu.Permissions)
            {
                string permissionName =
                    $"{menu.Key}.{submenu.Key}.{permission}";

                options.AddPolicy(permissionName, policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.AddRequirements(new AuthorizationRequirement(permissionName));
                });
            }
        }
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.Initialize(scope.ServiceProvider);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();
app.UsePermissionMiddleware();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TASK8.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("DefaultConnection is missing.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    // For this local assignment there is no email-confirmation service.
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

var app = builder.Build();

// Create only the three predefined roles. Never assign privileged roles at registration.
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    foreach (var role in new[] { "User", "Admin", "SuperAdmin" })
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(role));
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }

    // Development-only first SuperAdmin: set BootstrapSuperAdminEmail via User Secrets
    // AFTER registering that email normally. Remove the secret after promoting the account.
    if (app.Environment.IsDevelopment())
    {
        var email = builder.Configuration["BootstrapSuperAdminEmail"];
        if (!string.IsNullOrWhiteSpace(email))
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var user = await userManager.FindByEmailAsync(email);
            if (user != null && !await userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                var oldRoles = await userManager.GetRolesAsync(user);
                if (oldRoles.Count > 0)
                {
                    var removed = await userManager.RemoveFromRolesAsync(user, oldRoles);
                    if (!removed.Succeeded)
                        throw new InvalidOperationException(string.Join("; ", removed.Errors.Select(e => e.Description)));
                }
                var added = await userManager.AddToRoleAsync(user, "SuperAdmin");
                if (!added.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", added.Errors.Select(e => e.Description)));
            }
        }
    }
}

if (app.Environment.IsDevelopment()) app.UseMigrationsEndPoint();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
// Identity services are used by our own AccountController.
app.Run();

# TASK8 – University Cafeteria Supply Request Management System

ASP.NET Core MVC (.NET 10) / EF Core / SQL Server / ASP.NET Core Identity.

## Changes
- Keeps `IdentityUser`, the existing Identity schema and your migration history.
- Uses `AccountController` and custom Register/Login views, with full name kept in `AspNetUserClaims` (GivenName). Identity UserName is the email, and passwords are created with `CreateAsync(user, password)`.
- Registration automatically signs in the user, so Logout appears immediately. Login by email also works.
- Seeds exactly `User`, `Admin`, `SuperAdmin` roles after database migrations are applied.
- Product CRUD, supply-request CRUD with product quantities, server-side total calculation, approve/reject, and role-restricted user-role management.
- No explicit `[ValidateAntiForgeryToken]` attributes. ASP.NET Core Form Tag Helpers still generate anti-forgery tokens by default.

## First run
1. Open `TASK8.slnx` in Visual Studio, check `appsettings.json` DefaultConnection Server name (replace `localhost` with your actual SQL Server instance if needed).
2. Since original migrations have been kept, **do not delete/recreate them**. In Package Manager Console choose project `TASK8` and run `Update-Database` if you haven't already.
3. Build and launch. Roles are seeded at startup, after `Update-Database`.
4. Register an ordinary user at `/Account/Register`. Registration signs the user in automatically, and Logout appears in the navbar. Existing accounts created with plaintext `PasswordHash` may not log in; create a new test account or reset their password properly.
5. For the first **SuperAdmin**, register the intended email normally. In Visual Studio right-click the project > **Manage User Secrets** and set `{ "BootstrapSuperAdminEmail": "YOUR_EMAIL@example.com" }`. Restart the app once. The registered user becomes SuperAdmin in development only. **Remove the secret immediately** and restart. Never deploy this bootstrap mechanism enabled outside Development.
6. Log out and log in again to refresh the authentication cookie and role claims. Use `Manage Users` to make other users Admin. Only SuperAdmin can change roles.
7. Admin / SuperAdmin can add products. Users can create requests. Admin / SuperAdmin can approve/reject requests by changing their status.

## Notes
- For an assignment, no email-confirmation service is configured; the three `RequireConfirmed...` sign-in flags are false.
- `IdentityUser` doesn't contain FullName, so we save it as a user claim in `AspNetUserClaims` without introducing a User table or new migration.
- `RequestItem.Price` is a server-side price snapshot; totals are never accepted from browser input.
- The request form lists the products in the database, with a quantity field for each product.
- This source was assembled and reviewed in an environment without the .NET SDK or SQL Server, so a local Visual Studio build/runtime test is still required. See troubleshooting in project if a machine-specific SQL server name differs.

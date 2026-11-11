using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TASK8.Data;
using TASK8.Models;

namespace TASK8.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AdminController(UserManager<IdentityUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.Users = _userManager.Users.ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            ViewBag.User = user;
            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            return View(new UserRoleDTO { UserId = id });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserRoleDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();
            if (!new[] { "User", "Admin", "SuperAdmin" }.Contains(model.RoleName))
                return BadRequest("Invalid role.");
            var oldRoles = await _userManager.GetRolesAsync(user);
            if (oldRoles.Contains("SuperAdmin") && model.RoleName != "SuperAdmin" &&
                !await HasOtherSuperAdmin(user.Id))
            {
                TempData["Error"] = "Cannot remove the last SuperAdmin.";
                return RedirectToAction("Index");
            }
            if (oldRoles.Count > 0)
            {
                var removed = await _userManager.RemoveFromRolesAsync(user, oldRoles);
                if (!removed.Succeeded)
                {
                    TempData["Error"] = string.Join("; ", removed.Errors.Select(e => e.Description));
                    return RedirectToAction("Index");
                }
            }
            var added = await _userManager.AddToRoleAsync(user, model.RoleName);
            if (!added.Succeeded) TempData["Error"] = string.Join("; ", added.Errors.Select(e => e.Description));
            return RedirectToAction("Index");
        }

        private async Task<bool> HasOtherSuperAdmin(string currentId)
        {
            var admins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
            return admins.Any(u => u.Id != currentId);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction("Index");
            }
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") &&
                !await HasOtherSuperAdmin(user.Id))
            {
                TempData["Error"] = "Cannot delete the last SuperAdmin.";
                return RedirectToAction("Index");
            }
            using var transaction = await _context.Database.BeginTransactionAsync();
            var requests = await _context.SupplyRequests
                .Where(r => r.UserId == id).ToListAsync();
            _context.SupplyRequests.RemoveRange(requests);
            await _context.SaveChangesAsync();
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded) await transaction.CommitAsync();
            else TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            return RedirectToAction("Index");
        }
    }
}

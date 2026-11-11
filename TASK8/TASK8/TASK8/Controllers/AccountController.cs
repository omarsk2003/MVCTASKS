using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TASK8.Models;

namespace TASK8.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
                                  SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index() => RedirectToAction("Index", "Home");

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            if (!ModelState.IsValid) return View(register);

            if (await _userManager.FindByEmailAsync(register.Email) != null)
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View(register);
            }

            // Use email as the unique UserName. Store FullName in Identity's existing claims table.
            var newUser = new IdentityUser
            {
                UserName = register.Email,
                Email = register.Email
            };

            var result = await _userManager.CreateAsync(newUser, register.Password);
            if (result.Succeeded)
            {
                var role = await _userManager.AddToRoleAsync(newUser, "User");
                var name = role.Succeeded
                    ? await _userManager.AddClaimAsync(newUser,
                        new Claim(ClaimTypes.GivenName, register.FullName.Trim()))
                    : role;
                if (role.Succeeded && name.Succeeded)
                {
                    // Register completes sign-in too: the navbar immediately displays Logout.
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in role.Errors.Concat(name.Errors))
                    ModelState.AddModelError("", error.Description);
                // Undo partially-created account so Register can be retried cleanly.
                await _userManager.DeleteAsync(newUser);
                return View(register);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(register);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login() => View();

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            if (!ModelState.IsValid) return View(login);
            var user = await _userManager.FindByEmailAsync(login.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    user, login.Password, login.RememberMe, false);
                if (result.Succeeded) return RedirectToAction("Index", "Home");
                if (result.IsNotAllowed)
                    ModelState.AddModelError("", "Login not allowed. Check Identity sign-in settings.");
                else if (result.IsLockedOut)
                    ModelState.AddModelError("", "Account is locked.");
                else
                    ModelState.AddModelError("", "Invalid email or password.");
            }
            else ModelState.AddModelError("", "Invalid email or password.");
            return View(login);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}

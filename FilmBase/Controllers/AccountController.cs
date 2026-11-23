using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using FilmBase.Services;

namespace FilmBase.Controllers
{
    public class AccountController : Controller
    {
        private readonly FirebaseAuthService _authService;

        public AccountController(FirebaseAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var result = await _authService.LoginAsync(email, password);
            if (!result.Success)
            {
                ViewBag.Error = result.Error;
                return View();
            }

            await SignInUserAsync(result.Uid!, email, result.Username!);
            return RedirectToAction("Index", "Movies");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string email, string password)
        {
            var result = await _authService.RegisterAsync(email, password, username);
            if (!result.Success)
            {
                ViewBag.Error = result.Error;
                return View();
            }

            await SignInUserAsync(result.Uid!, email, username);
            return RedirectToAction("Index", "Movies");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Movies");
        }

        private async Task SignInUserAsync(string uid, string email, string username)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, uid),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));
        }
    }
}
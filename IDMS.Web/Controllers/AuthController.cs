using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IDMS.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IDMS.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Brand");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Brand");
            }
            return View();
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Login([FromBody] AuthRequest request)
        {
            var (success, token, userEmail, error) = await _auth.LoginAsync(request.Email, request.Password);

            if (!success)
            {
                return Json(new { status = "Error", message = error ?? "Login Failed" });
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, userEmail ?? request.Email),
                new("JwtToken", token ?? "")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return Json(new { status = "Success", data = new { email = userEmail ?? request.Email }, message = "Login sukses" });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Register([FromBody] AuthRequest request)
        {
            var (success, error) = await _auth.RegisterAsync(request.Email, request.Password, request.fullName);

            if (!success)
            {
                return Json(new { status = "Error", message = error ?? "Login Failed" });
            }

            return Json(new { status = "Success", data = new { email = request.Email }, message = "Register sukses" });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<JsonResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Json(new { status = "Success", message = "Logged Out!" });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }

    public class AuthRequest
    {
        public string fullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
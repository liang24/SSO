using System;
using System.Collections.Generic;
using System.Security.Claims;
using App1.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace App1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private static Dictionary<string, string> _accounts;

        static AccountController()
        {
            _accounts = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _accounts.Add("Foo", "password");
            _accounts.Add("Bar", "password");
            _accounts.Add("Baz", "password");
        }

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginModel model = new LoginModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (_accounts.TryGetValue(model.UserName, out var pwd) && pwd == model.Password)
            {
                var claimsIdentity = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, model.UserName) }, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return Redirect("/");
            }
            else
            {
                model.ErrorMessage = "Invalid user name or password!";

                return await Task.Run(() => View(model));
            }
        }

        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation("User {Name} logged out at {Time}.",
                User.Identity.Name, DateTime.UtcNow);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/");
        }
    }
}

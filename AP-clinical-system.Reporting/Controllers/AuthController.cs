using AP_clinical_system.Reporting.Dtos;
using AP_clinical_system.Reporting.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AP_clinical_system.Reporting.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiClient _apiClient;

        public AuthController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        //login GET
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //Login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginReq model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var response = await _apiClient.LoginAsync(model);

            if (response == null)
            {
                ModelState.AddModelError("", "Invalid login or only Clinic Manager can access.");
                return View(model);
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(response.Token);

            var claims = jwtToken.Claims.ToList();

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = response.Expiration,
                IsPersistent = false
            };

            authProperties.StoreTokens(new[]
            {
                new AuthenticationToken
                {
                    Name = "access_token",
                    Value = response.Token
                }
            });

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authProperties
            );

            return RedirectToAction("Dashboard", "Reports");
        }

        //Logout 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
    }
}

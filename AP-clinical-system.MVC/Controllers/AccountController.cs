using Microsoft.AspNetCore.Mvc;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using AP_clinical_system.ViewModels;
using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using System.Security.Claims;
using AP_clinical_system.Models.Enums;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace AP_clinical_system.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<system_user> _userManager;
        private readonly SignInManager<system_user> _signInManager;
        private readonly AP_Context _context;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<system_user> userManager,
            SignInManager<system_user> signInManager,
            AP_Context context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _configuration = configuration;
        }

        //GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Auth/register.cshtml");
        }

        //POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            { 
                var userNo = await GeneralHelper.GetAutonumber(_context, "system_user");
                var patientRecordNo = await GeneralHelper.GetAutonumber(_context, "patient_information");

                var user = new system_user
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone_Number,

                    first_name = model.First_Name,
                    last_name = model.Last_Name,
                    cpr = model.CPR,

                    createdon = DateTime.Now,
                    inactive = false,

                    user_role = (int)user_role.patient, //the registered role will always be "Paient"
                    user_no = userNo
                }; 

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //inserting in patient_information table
                    var patient = new patient_information
                    {
                        id = Guid.NewGuid(),
                        inactive = false,
                        createdon = DateTime.Now,
                        createdby = user.Id,

                        record_no = patientRecordNo,
                        system_user_ref = user.Id,
                    };

                    _context.patient_informations.Add(patient);
                    await _context.SaveChangesAsync();

                    await _userManager.AddClaimAsync(user, new Claim("FirstName", model.First_Name));
                    await _userManager.AddClaimAsync(user, new Claim("LastName", model.Last_Name));
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "patient"));

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Patient"); 
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("~/Views/Auth/register.cshtml", model);
        }

        //GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/Auth/login.cshtml");
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("~/Views/Auth/login.cshtml", model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    user.UserName,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false
                );

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    // Redirect based on user_role
                    return (user_role?)user.user_role switch
                    {
                        user_role.patient => RedirectToAction("Index", "Patient"),
                        user_role.doctor => RedirectToAction("Index", "Doctor"),
                        user_role.receptionist => RedirectToAction("Index", "Receptionist"),
                        user_role.clinic_manager => RedirectToAction("Index", "Manager"),
                        user_role.system_admin => RedirectToAction("Index", "Admin"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Account locked out. Try again later.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View("~/Views/Auth/login.cshtml", model);
        }

        //POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // POST: /api/Account/Login
        [HttpPost("api/Account/Login")]
        public async Task<IActionResult> ApiLogin([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || user.inactive == true)
            {
                return Unauthorized(new { message = "Invalid credentials or account is inactive." });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            var roleString = ((user_role?)user.user_role)?.ToString() ?? "unknown";

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FirstName", user.first_name ?? ""),
                new Claim("LastName", user.last_name ?? ""),
                new Claim(ClaimTypes.Role, roleString)
            };

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"]));

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expires,
                role = roleString
            });
        }

        //GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}

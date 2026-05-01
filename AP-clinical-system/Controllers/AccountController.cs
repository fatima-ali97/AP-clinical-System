using Microsoft.AspNetCore.Mvc;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using AP_clinical_system.ViewModels;
using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using System.Security.Claims;

namespace AP_clinical_system.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<system_user> _userManager;
        private readonly SignInManager<system_user> _signInManager;
        private readonly AP_Context _context;

        public AccountController(
            UserManager<system_user> userManager,
            SignInManager<system_user> signInManager,
            AP_Context context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

                    user_role = 1, //the registered role will always be "Paient"
                    user_no = userNo
                }; 

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    //inserting in patient_information table
                    await _userManager.AddToRoleAsync(user, "Patient");

                    var patient = new patient_information
                    {
                        id = Guid.NewGuid(),
                        inactive = false,
                        createdon = DateTime.Now,
                        createdby = user.Id,

                        record_no = patientRecordNo,
                        system_user_ref = user.Id,
                        system_user_lookup_to = "system_user"
                    };

                    await _userManager.AddToRoleAsync(user, "Patient"); //the registered user is always a patient
                    _context.patient_informations.Add(patient);
                    await _context.SaveChangesAsync();

                    await _userManager.AddClaimAsync(user, new Claim("FirstName", model.First_Name));
                    await _userManager.AddClaimAsync(user, new Claim("LastName", model.Last_Name));

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home"); 
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

                    return RedirectToAction("Index", "Patient");
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

        //GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}

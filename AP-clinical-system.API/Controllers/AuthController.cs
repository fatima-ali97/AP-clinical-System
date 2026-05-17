using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using AP_clinical_system.Reporting.ViewModels;
using System.ComponentModel.Design.Serialization;

namespace AP_clinical_system.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<system_user> _userManager;
        private readonly SignInManager<system_user> _signInManager;
        private readonly AP_Context _context;
        private readonly IConfiguration _configuration;

        public AuthController(
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

        // POST: api/Auth/ReportingAppLogin
        [HttpPost]
        public async Task<IActionResult> ReportingAppLogin([FromBody] ReportingLoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Find user by email
            var user = _context.system_users.First(u => u.Email == model.Email);
            if (user == null || user.inactive == true)
            {
                return Unauthorized(new { message = "Invalid credentials or account is inactive." });
            }

            // Verify password
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new { message = "Invalid credentials." });
            }

            // Verify user is a clinic manager using GeneralHelper
            var isClinicManager = GeneralHelper.VerifyUserType(_context, user.Id, (int)user_role.clinic_manager);
            if (!isClinicManager)
            {
                return Unauthorized(new { message = "Access denied. Only clinic managers can access the reporting app." });
            }

            // Generate JWT with user_role claim (as int from our system_user table, not the IdentityUser roles)
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FirstName", user.first_name ?? ""),
                new Claim("LastName", user.last_name ?? ""),
                new Claim("UserRole", user.user_role.ToString()),
                new Claim(ClaimTypes.Role, nameof(user_role.clinic_manager))
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
                role = nameof(user_role.clinic_manager),
                userRole = user.user_role
            });
        }
    }
}

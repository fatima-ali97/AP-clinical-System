using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using AP_clinical_system.Models.Enums;

namespace AP_clinical_system.MVC.Models
{
    public static class DataFixer
    {
        public static async Task FixExistingUsersAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<system_user>>();
            var context = scope.ServiceProvider.GetRequiredService<AP_Context>();

            // Find all users missing NormalizedEmail or PasswordHash
            var brokenUsers = await context.system_users
                .Where(u => u.NormalizedEmail == null || u.PasswordHash == null || u.SecurityStamp == null || u.UserName == null || u.NormalizedUserName == null)
                .ToListAsync();

            if (!brokenUsers.Any())
            {
                return;
            }

            foreach (var user in brokenUsers)
            {
                // Ensure email is set as UserName if missing
                if (string.IsNullOrEmpty(user.UserName) && !string.IsNullOrEmpty(user.Email))
                {
                    user.UserName = user.Email;
                }

                // Update normalization fields
                if (!string.IsNullOrEmpty(user.Email))
                {
                    user.NormalizedEmail = userManager.NormalizeEmail(user.Email);
                }
                
                if (!string.IsNullOrEmpty(user.UserName))
                {
                    user.NormalizedUserName = userManager.NormalizeName(user.UserName);
                }

                // Generate Security Stamp if missing
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    user.SecurityStamp = Guid.NewGuid().ToString();
                }

                // Fix password hashing
                // As requested, set all fixed accounts' passwords to "123"
                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, "123");

                // Ensure Identity role matches user_role field
                if (user.user_role.HasValue)
                {
                    var roleEnum = (user_role)user.user_role.Value;
                    string roleName = roleEnum switch
                    {
                        user_role.patient => "Patient",
                        user_role.doctor => "Doctor",
                        user_role.receptionist => "Receptionist",
                        user_role.clinic_manager => "ClinicManager",
                        user_role.system_admin => "SystemAdmin",
                        _ => null
                    };

                    if (!string.IsNullOrEmpty(roleName))
                    {
                        if (!await userManager.IsInRoleAsync(user, roleName))
                        {
                            await userManager.AddToRoleAsync(user, roleName);
                        }
                    }
                }

                // Update user
                await userManager.UpdateAsync(user);
            }
        }
    }
}

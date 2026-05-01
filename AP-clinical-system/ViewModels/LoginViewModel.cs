using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.ViewModels
{
    public class LoginViewModel
    {
        //Email
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        //Password
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        //Remember Me
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.ViewModels
{
    public class RegisterViewModel
    {
        //First Name
        [Required]
        [Display(Name = "First Name")]
        public required string First_Name { get; set; }

        //Last Name
        [Required]
        [Display(Name = "Last Name")]
        public required string Last_Name { get; set; }

        //CPR
        [Required]
        [Display(Name = "CPR")]
        public required string CPR { get; set; }

        //Phone Number 
        [Required]
        [Display(Name = "Phone Number")]
        public required string Phone_Number { get; set; }

        //Email
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        //Password
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least {2} characters long.")]
        [Display(Name = "Password")]
        public required string Password { get; set; }

        //Confirm Password
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Confirm Password")]
        public required string ConfirmPassword { get; set; }

    }
}

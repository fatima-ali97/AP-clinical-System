using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.Reporting.Dtos
{
    public class LoginReq
    {
        //Email
        [Required]
        [EmailAddress]
        public required string Email { get; set; } = string.Empty;

        //Password
        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; } = string.Empty;

    }
}

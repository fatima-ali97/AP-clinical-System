using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.Reporting.ViewModels
{
    public class ReportingLoginViewModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public required string Password { get; set; }
    }
}

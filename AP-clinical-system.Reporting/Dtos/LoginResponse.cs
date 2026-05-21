namespace AP_clinical_system.Reporting.Dtos
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expiration { get; set; }

        public string Role { get; set; } = string.Empty;

        public int? UserRole { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;

namespace AP_clinical_system.Models.Entities
{
    public class system_user : IdentityUser<Guid>
    {
        // Identity User already provides some fields
        // Custom fields:
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }
        
        public string? user_no { get; set; }
        public string? cpr { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public int? user_role { get; set; }
    }
}

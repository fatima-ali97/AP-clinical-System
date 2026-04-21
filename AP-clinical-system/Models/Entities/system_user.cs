namespace AP_clinical_system.Models.Entities
{
    public class system_user
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }
        
        public string? user_no { get; set; }
        public string? cpr { get; set; }
        public string? firt_name { get; set; }
        public string? last_name { get; set; }
        public string? email { get; set; }
        public string? phone_number { get; set; }
        public string? hashed_password { get; set; }
        public int? user_role { get; set; }
    }
}

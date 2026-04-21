namespace AP_clinical_system.Models.Entities
{
    public class notification
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public Guid system_user_ref { get; set; }
        public string system_user_lookup_to { get; set; } = "system_user";
        public string title { get; set; }
        public string message { get; set; }
        public bool is_read { get; set; }
    }
}

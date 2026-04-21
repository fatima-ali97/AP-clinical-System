namespace AP_clinical_system.Models.Entities
{
    public class doctor_leave
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string? record_no { get; set; }
        public Guid? doctor_information_ref { get; set; }
        public string? doctor_information_lookup_to { get; set; } = "doctor_information";
        public DateOnly? start_date { get; set; }
        public DateOnly? end_date { get; set; }
        public string? reason { get; set; }
        public int? leave_type { get; set; }
    }
}

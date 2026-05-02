namespace AP_clinical_system.Models.Entities
{
    public class autonumber
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string? entity_name { get; set; }
        public string? field_name { get; set; }
        public string? pattern { get; set; }
        public int? last_number { get; set; }
    }
}

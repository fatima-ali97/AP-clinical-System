namespace AP_clinical_system.Models.Entities
{
    public class doctor_schedule
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public Guid doctor_information_ref { get; set; }
        public string doctor_information_lookup_to { get; set; }
        public string? sunday_start { get; set; }
        public string? sunday_end { get; set; }

        public string? monday_start { get; set; }
        public string? monday_end { get; set; }

        public string? tuesday_start { get; set; }
        public string? tuesday_end { get; set; }

        public string? wednesday_start { get; set; }
        public string? wednesday_end { get; set; }

        public string? thursday_start { get; set; }
        public string? thursday_end { get; set; }

        public string? friday_start { get; set; }
        public string? friday_end { get; set; }
        
        public string? saturday_start { get; set; }
        public string? saturday_end { get; set; }
    }
}

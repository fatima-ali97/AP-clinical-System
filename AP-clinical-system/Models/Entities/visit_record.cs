namespace AP_clinical_system.Models.Entities
{
    public class visit_record
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string record_no { get; set; }
        public Guid appointment_ref { get; set; }
        public string appointment_lookup_to { get; set; } = "appointment";
        public string doctor_notes { get; set; }
        public string diagnosis { get; set; }
        public string prescribed_treatment { get; set; }
    }
}

namespace AP_clinical_system.Models.Entities
{
    public class appointment
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string appointment_no { get; set; }
        public string appointment_reason { get; set; }
        public Guid patient_ref { get; set; }
        public string patient_lookup_to { get; set; }
        public Guid doctor_ref { get; set; }
        public string doctor_lookup_to { get; set; }
        public Guid receptionist_ref { get; set; }
        public string receptionist_lookup_to { get; set; }
        public Guid specialization_ref { get; set; }
        public string specialization_lookup_to { get; set; }
        public DateOnly date { get; set; }
        public int time_slot { get; set; }
        public int status { get; set; }
    }
}

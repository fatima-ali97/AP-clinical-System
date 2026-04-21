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
        public string patient_lookup_to { get; set; } = "patient_information";
        public Guid doctor_ref { get; set; }
        public string doctor_lookup_to { get; set; } = "doctor_information";
        public Guid receptionist_ref { get; set; }
        public string receptionist_lookup_to { get; set; } = "receptionist_information";
        public Guid specialization_ref { get; set; }
        public string specialization_lookup_to { get; set; } = "doctor_specialization";
        public DateOnly date { get; set; }
        public int appointment_time_slot { get; set; }
        public int appointment_status { get; set; }
    }
}

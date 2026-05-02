namespace AP_clinical_system.Models.Entities
{
    public class prescription
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string? prescription_no { get; set; }
        public string? medicine_name { get; set; }
        public string? medicine_dosage { get; set; }
        public string? medicine_frequency_duration { get; set; }
        public Guid? appointment_ref { get; set; }
        public string? appointment_lookup_to { get; set; } = "appointment";
        public Guid? patient_ref { get; set; }
        public string? patient_lookup_to { get; set; } = "patient_information";
        public Guid? doctor_ref { get; set; }
        public string? doctor_lookup_to { get; set; } = "doctor_information";
    }
}

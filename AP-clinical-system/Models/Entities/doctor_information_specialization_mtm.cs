namespace AP_clinical_system.Models.Entities
{
    public class doctor_information_specialization_mtm
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string record_no { get; set; }
        public Guid doctor_information_ref { get; set; } 
        public string doctor_information_lookup_to { get; set; } = "doctor_information";
        public Guid doctor_specialization_ref { get; set; }
        public string doctor_specialization_lookup_to { get; set; } = "doctor_specialization";
    }
}

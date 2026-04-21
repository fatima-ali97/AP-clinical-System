namespace AP_clinical_system.Models.Entities
{
    public class patient_information
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set;}

        public string record_no { get; set; }
        public Guid system_user_ref { get; set; }
        public string system_user_lookup_to { get; set; } = "system_user";
    }
}

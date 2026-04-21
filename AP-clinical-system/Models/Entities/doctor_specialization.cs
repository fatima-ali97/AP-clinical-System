namespace AP_clinical_system.Models.Entities
{
    public class doctor_specialization
    {
        public Guid id { get; set; }
        public bool? inactive { get; set; }
        public DateTime createdon { get; set; }
        public Guid? createdby { get; set; }
        public DateTime? modifiedon { get; set; }
        public Guid? modifiedby { get; set; }

        public string record_no { get; set; }
        public string specialization_name { get; set; }
        public string specialization_details { get; set; }
    }
}

public class BookAppointmentViewModel
{
    public Guid? patient_ref { get; set; }
    public Guid? doctor_ref { get; set; }
    public Guid? specialization_ref { get; set; }
    public DateOnly? date { get; set; }
    public int? appointment_time_slot { get; set; }
    public string? appointment_reason { get; set; }
}
namespace AP_clinical_system.Reporting.Dtos
{
    public class ReportDto
    {
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int MissedAppointments { get; set; }

        public double CancelledRate { get; set; }
        public double MissedRate { get; set; }

        public List<DoctorAppointmentsDto> DoctorWorkloads { get; set; } = new();

    }

    public class DoctorAppointmentsDto //each doc total appointments
    {
        public string DoctorName { get; set; } = string.Empty;
        public int TotalAppointments { get; set; }
    }
}

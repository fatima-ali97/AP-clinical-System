using System;
using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.ViewModels
{
    public class BookAppointmentViewModel
    {
        [Required]
        public Guid doctor_ref { get; set; }

        [Required]
        public Guid specialization_ref { get; set; }

        [Required]
        public DateTime date { get; set; }

        [Required]
        public int appointment_time_slot { get; set; }

        public string? appointment_reason { get; set; }
    }
}

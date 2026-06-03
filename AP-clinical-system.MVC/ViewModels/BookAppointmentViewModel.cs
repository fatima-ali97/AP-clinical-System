using System;
using System.ComponentModel.DataAnnotations;

namespace AP_clinical_system.ViewModels
{
    public class BookAppointmentViewModel
    {
        [Required(ErrorMessage = "Patient is required.")]
        public Guid? patient_ref { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public Guid? doctor_ref { get; set; }

        [Required(ErrorMessage = "Specialization is required.")]
        public Guid? specialization_ref { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.Date)]
        public DateTime? date { get; set; }

        [Required(ErrorMessage = "Time slot is required.")]
        [Range(0, 2359, ErrorMessage = "Please enter a valid military time slot format (HHMM).")]
        public int? appointment_time_slot { get; set; }

        [StringLength(500, ErrorMessage = "The reason cannot exceed 500 characters.")]
        public string? appointment_reason { get; set; }
    }
}
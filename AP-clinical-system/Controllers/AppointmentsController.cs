using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using static AP_clinical_system.Models.GeneralHelper;

namespace AP_clinical_system.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly AP_Context context;

        public AppointmentsController(AP_Context _context)
        {
            context = _context;
        }

        [HttpGet]
        public IActionResult GetAllSpecializations()
        {
            var specializations = context.doctor_specializations
            .Where(s => s.inactive != true)
            .Select(s => new
            {
                s.id,
                s.specialization_name
            })
            .ToList();

            return Ok(specializations);
        }

        [HttpPost]
        public IActionResult GetDoctorsBySpecialization([FromBody] JsonElement body)
        {
            var specializationID = body.GetProperty("specializationID").GetGuid();

            var specialization = context.doctor_specializations
                .Where(s => s.id == specializationID && s.inactive != true)
                .FirstOrDefault();

            if (specialization == null)
            {
                return NotFound("Specialization not found.");
            }

            var doctors = GetAllDoctors();

            var filteredDoctors = doctors
                .Where(d => d.Specializations.Any(s => s.id == specializationID))
                .ToList();

            return Ok(filteredDoctors);
        }

        [HttpPost]
        public IActionResult GetAvailableDatesAndTimesByDoctorID([FromBody] JsonElement body)
        {
            var doctorID = body.GetProperty("doctorID").GetGuid();

            var doctor = context.system_users
                .Where(u => u.id == doctorID && u.user_role == (int)user_role.doctor && u.inactive != true)
                .FirstOrDefault();

            if (doctor == null)
            {
                return NotFound("Doctor not found.");
            }

            var doctor_information = context.doctor_informations
                .Where(d => d.system_user_ref == doctorID && d.inactive != true)
                .FirstOrDefault();

            if (doctor_information == null)
            {
                return NotFound("Doctor information not found.");
            }

            var doctor_schedule = context.doctor_schedules
                .Where(s => s.doctor_information_ref == doctor_information.id && s.inactive != true)
                .FirstOrDefault();

            if (doctor_schedule == null)
            {
                return NotFound("Doctor schedule not found.");
            }

            var doctor_leaves = context.doctor_leaves
                .Where(l => l.doctor_information_ref == doctor_information.id
                    && l.end_date >= DateOnly.FromDateTime(DateTime.Today)
                    && l.inactive != true)
                .ToList();

            var existingAppointments = context.appointments
                .Where(a => a.doctor_ref == doctor_information.id
                    && a.date >= DateOnly.FromDateTime(DateTime.Today)
                    && a.inactive != true
                    && a.appointment_status != (int)appointment_status.cancelled)
                .ToList();

            // Build a lookup of time slot enum values to their corresponding times
            // Each slot is 30 min: 1000 = 08:00, 1001 = 08:30, ..., 1018 = 17:00
            var allTimeSlots = Enum.GetValues<appointment_time_slot>()
                .Select(s => new
                {
                    EnumValue = (int)s,
                    Hours = 8 + ((int)s - 1000) / 2,
                    Minutes = ((int)s - 1000) % 2 == 0 ? 0 : 30
                })
                .ToList();

            var today = DateOnly.FromDateTime(DateTime.Today);
            int daysToGenerate = 30;
            var result = new List<object>();

            for (int i = 0; i < daysToGenerate; i++)
            {
                var currentDate = today.AddDays(i);

                // Get the schedule start/end for this day of the week
                var (startTime, endTime) = GetScheduleForDay(doctor_schedule, currentDate.DayOfWeek);

                // Skip if doctor doesn't work on this day
                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                    continue;

                // Skip if doctor is on leave this day
                bool isOnLeave = doctor_leaves.Any(l =>
                    l.start_date <= currentDate && l.end_date >= currentDate);
                if (isOnLeave)
                    continue;

                // Parse schedule start/end into hours and minutes
                var startParts = startTime.Split(':');
                int startHour = int.Parse(startParts[0]);
                int startMinute = int.Parse(startParts[1]);

                var endParts = endTime.Split(':');
                int endHour = int.Parse(endParts[0]);
                int endMinute = int.Parse(endParts[1]);

                // Filter time slots that fall within the doctor's working hours for this day
                var availableSlots = allTimeSlots
                    .Where(s =>
                    {
                        int slotTotal = s.Hours * 60 + s.Minutes;
                        int scheduleStart = startHour * 60 + startMinute;
                        int scheduleEnd = endHour * 60 + endMinute;
                        return slotTotal >= scheduleStart && slotTotal < scheduleEnd;
                    })
                    .Select(s => s.EnumValue)
                    .ToList();

                // Remove slots that are already booked
                var bookedSlots = existingAppointments
                    .Where(a => a.date == currentDate && a.appointment_time_slot.HasValue)
                    .Select(a => a.appointment_time_slot.Value)
                    .ToHashSet();

                availableSlots = availableSlots
                    .Where(s => !bookedSlots.Contains(s))
                    .ToList();

                if (availableSlots.Any())
                {
                    result.Add(new
                    {
                        date = currentDate,
                        availableTimeSlots = availableSlots
                    });
                }
            }

            return Ok(result);
        }



        private (string? start, string? end) GetScheduleForDay(doctor_schedule schedule, DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Sunday => (schedule.sunday_start, schedule.sunday_end),
                DayOfWeek.Monday => (schedule.monday_start, schedule.monday_end),
                DayOfWeek.Tuesday => (schedule.tuesday_start, schedule.tuesday_end),
                DayOfWeek.Wednesday => (schedule.wednesday_start, schedule.wednesday_end),
                DayOfWeek.Thursday => (schedule.thursday_start, schedule.thursday_end),
                DayOfWeek.Friday => (schedule.friday_start, schedule.friday_end),
                DayOfWeek.Saturday => (schedule.saturday_start, schedule.saturday_end),
                _ => (null, null)
            };
        }


        private List<DoctorObject> GetAllDoctors()
        {

            var doctors = new List<DoctorObject>();

            var doctorIDs = context.system_users
                .Select(u => new
                {
                    u.id,
                    u.user_role,
                    u.inactive
                })
                .Where(u => u.user_role == (int)user_role.doctor && u.inactive != true)
                .ToList();

            foreach (var doctor in doctorIDs)
            {
                var doctorObject = GeneralHelper.GetDoctorObjectByID(context, doctor.id);
                if (doctorObject.Success)
                {
                    doctors.Add(doctorObject);
                }
            }

            return doctors;
        }
    }
}

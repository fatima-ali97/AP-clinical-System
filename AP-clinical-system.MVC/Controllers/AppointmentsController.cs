using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AP_clinical_system.Models;
using static AP_clinical_system.Models.GeneralHelper;

namespace AP_clinical_system.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly AP_Context context;

        public AppointmentsController(AP_Context _context)
        {
            context = _context;
        }

        // GET /Appointments/getAllSpecializations
        [HttpGet]
        public IActionResult getAllSpecializations()
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

     
        [HttpGet]
        public async Task<IActionResult> GetDoctorsBySpecialization(Guid specializationId)
        {
            // get specialization
            var specialization = await context.doctor_specializations
                .Where(s => s.id == specializationId && s.inactive != true)
                .FirstOrDefaultAsync();
            
            // verify it exists
            if (specialization == null)
            {
                return NotFound("Specialization ID provided does not belong to any active specialization.");
            }

            // get all doctors who specialize in the specialization
            var result = GeneralHelper.GetAllDoctors(context)
                .Where(d => d.Specializations.Any(s => s.id == specializationId))
                .Select(d => new
                {
                    d.id,
                    userId = d.id,
                    name = $"Dr. {GeneralHelper.GetUserFullNameByID(context, d.id)}"
                });

            return Json(result);
        }

        // POST /Appointments/GetAvailableDatesAndTimesByDoctorID
        [HttpPost]
        public IActionResult GetAvailableDatesAndTimesByDoctorID(Guid doctorID)
        {
            var doctor = context.system_users
                .Where(u => u.Id == doctorID && u.user_role == (int)user_role.doctor && u.inactive != true)
                .FirstOrDefault();

            if (doctor == null)
                return NotFound("Doctor not found.");

            var doctor_information = context.doctor_informations
                .Where(d => d.system_user_ref == doctorID && d.inactive != true)
                .FirstOrDefault();

            if (doctor_information == null)
                return NotFound("Doctor information not found.");

            var doctor_schedule = context.doctor_schedules
                .Where(s => s.doctor_information_ref == doctor_information.id && s.inactive != true)
                .FirstOrDefault();

            if (doctor_schedule == null)
                return NotFound("Doctor schedule not found.");

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

                var (startTime, endTime) = GetScheduleForDay(doctor_schedule, currentDate.DayOfWeek);

                if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
                    continue;

                bool isOnLeave = doctor_leaves.Any(l =>
                    l.start_date <= currentDate && l.end_date >= currentDate);
                if (isOnLeave)
                    continue;

                var startParts = startTime.Split(':');
                int startHour = int.Parse(startParts[0]);
                int startMinute = int.Parse(startParts[1]);

                var endParts = endTime.Split(':');
                int endHour = int.Parse(endParts[0]);
                int endMinute = int.Parse(endParts[1]);

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

                var bookedSlots = existingAppointments
                    .Where(a => a.date == currentDate && a.appointment_time_slot.HasValue)
                    .Select(a => a.appointment_time_slot!.Value)
                    .ToHashSet();

                availableSlots = availableSlots
                    .Where(s => !bookedSlots.Contains(s))
                    .ToList();

                if (availableSlots.Any())
                {
                    result.Add(new
                    {
                        date = currentDate.ToString("yyyy-MM-dd"),
                        availableTimeSlots = availableSlots
                    });
                }
            }

            return Ok(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(
            Guid specialization_ref,
            Guid doctor_ref,
            DateOnly date,
            int appointment_time_slot,
            string? appointment_reason)
        {
            var currentUser = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

            if (currentUser == null)
                return Json(new { success = false, error = "User not found." });

            var patientInfo = await context.patient_informations
                .FirstOrDefaultAsync(p => p.system_user_ref == currentUser.Id && p.inactive != true);

            if (patientInfo == null)
                return Json(new { success = false, error = "Patient record not found." });

            bool slotTaken = await context.appointments.AnyAsync(a =>
                a.doctor_ref == doctor_ref &&
                a.date == date &&
                a.appointment_time_slot == appointment_time_slot &&
                a.inactive != true);

            if (slotTaken)
                return Json(new { success = false, error = "That time slot is already booked. Please choose another." });

            var apptNo = await GeneralHelper.GetAutonumber(context, "appointment");

            var now = DateTime.UtcNow;
            var appt = new appointment
            {
                id = Guid.NewGuid(),
                appointment_no = apptNo,
                appointment_reason = appointment_reason,
                patient_ref = patientInfo.id,
                doctor_ref = doctor_ref,
                specialization_ref = specialization_ref,
                date = date,
                appointment_time_slot = appointment_time_slot,
                appointment_status = (int)appointment_status.requested,
                createdon = now,
                createdby = currentUser.Id,
                modifiedon = now,
                modifiedby = currentUser.Id,
                inactive = false
            };

            context.appointments.Add(appt);
            await context.SaveChangesAsync();

            return Json(new { success = true, appointment_no = appt.appointment_no });
        }

        // POST /Appointments/BookAppointment
        [HttpPost]
        public async Task<IActionResult> BookAppointment(ViewModels.Patient.BookAppointmentViewModel model)
        {
            try
            {
                var patientId = GeneralHelper.GetUserIdByJWT(User);

                var patientUserCheck = GeneralHelper.VerifyUserType(context, patientId, (int)user_role.patient);
                if (!patientUserCheck)
                    return BadRequest("Current User is not a Patient");


                var patientInfo = await context.patient_informations
                    .FirstOrDefaultAsync(p => p.system_user_ref == patientId && p.inactive != true);

                if (patientInfo == null)
                    return BadRequest("Patient information record not found.");

                var id = Guid.NewGuid();
                var createdon = DateTime.Now;
                var createdby = patientId;
                var modifiedon = createdon;
                var modifiedby = createdby;
                var inactive = false;

                var appointment_no = await GeneralHelper.GetAutonumber(context, "appointment");

                var doctorUserCheck = GeneralHelper.VerifyUserType(context, model.doctor_ref, (int)user_role.doctor);
                if (!doctorUserCheck)
                    return BadRequest("Doctor ID provided does not belong to a doctor");

                var newAppointment = new appointment
                {
                    id = id,
                    inactive = inactive,
                    createdon = createdon,
                    createdby = createdby,
                    modifiedon = modifiedon,
                    modifiedby = modifiedby,
                    appointment_no = appointment_no,
                    appointment_reason = model.appointment_reason ?? "",
                    patient_ref = patientInfo.id,
                    doctor_ref = model.doctor_ref,
                    specialization_ref = model.specialization_ref,
                    date = model.date,
                    appointment_time_slot = model.appointment_time_slot,
                    appointment_status = (int)appointment_status.requested
                };

                context.Add(newAppointment);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return RedirectToAction("Index", "Patient");
        }
        // POST /Appointments/ConfirmAppointment
        [HttpPost]
        public IActionResult ConfirmAppointment(Guid appointment_id)
        {
            try
            {
                var appointment = context.appointments
                    .FirstOrDefault(a => a.id == appointment_id && a.inactive != true);

                if (appointment == null)
                    return BadRequest("Appointment Not Found or is Inactive");

                var receptionistId = GeneralHelper.GetUserIdByJWT(User);

                var receptionistUserCheck = GeneralHelper.VerifyUserType(context, receptionistId, (int)user_role.receptionist);
                if (!receptionistUserCheck)
                    return BadRequest("User is not a receptionist");

                appointment.receptionist_ref = receptionistId;
                appointment.appointment_status = (int)appointment_status.confirmed;
                appointment.modifiedon = DateTime.UtcNow;
                appointment.modifiedby = receptionistId;

                context.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            return RedirectToAction("Index", "Receptionist");
        }

        private static (string? start, string? end) GetScheduleForDay(doctor_schedule schedule, DayOfWeek day)
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
        
    }
}

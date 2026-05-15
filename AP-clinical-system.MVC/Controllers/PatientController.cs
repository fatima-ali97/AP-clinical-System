using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using AP_clinical_system.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AP_clinical_system.Controllers
{
    public class PatientController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;

        public PatientController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            // ── Appointments ──────────────────────────────────────────────
            var appointments = await _context.appointments
                .Where(a => a.patient_ref == patientInfo.id && a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

            // ── Doctor IDs from appointments ──────────────────────────────
            var doctorIds = appointments
                .Where(a => a.doctor_ref.HasValue)
                .Select(a => a.doctor_ref!.Value)
                .Distinct()
                .ToList();

            var doctorInfos = await _context.doctor_informations
                .Where(d => doctorIds.Contains(d.id) && d.inactive != true)
                .ToListAsync();

            var doctorUserIds = doctorInfos
                .Where(d => d.system_user_ref.HasValue)
                .Select(d => d.system_user_ref!.Value)
                .ToList();

            var doctorUsers = await _context.Users
                .Where(u => doctorUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var doctorInfoToUser = doctorInfos
                .Where(d => d.system_user_ref.HasValue && doctorUsers.ContainsKey(d.system_user_ref!.Value))
                .ToDictionary(d => d.id, d => doctorUsers[d.system_user_ref!.Value]);

            // ── My Doctors (distinct + completed visit count) ─────────────
            var myDoctors = appointments
                .Where(a => a.doctor_ref.HasValue)
                .GroupBy(a => a.doctor_ref!.Value)
                .Select(g => new
                {
                    DoctorInfoId = g.Key,
                    TotalVisits = g.Count(a => a.appointment_status == (int)appointment_status.completed),
                    User = doctorInfoToUser.ContainsKey(g.Key) ? doctorInfoToUser[g.Key] : null
                })
                .Where(d => d.User != null)
                .ToList();

            // ── Prescriptions ─────────────────────────────────────────────
            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

            // Fetch any extra doctors referenced in prescriptions but not in appointments
            var prescriptionDoctorIds = prescriptions
                .Where(p => p.doctor_ref.HasValue)
                .Select(p => p.doctor_ref!.Value)
                .Distinct()
                .Except(doctorIds)
                .ToList();

            if (prescriptionDoctorIds.Any())
            {
                var extraDoctorInfos = await _context.doctor_informations
                    .Where(d => prescriptionDoctorIds.Contains(d.id) && d.inactive != true)
                    .ToListAsync();

                var extraUserIds = extraDoctorInfos
                    .Where(d => d.system_user_ref.HasValue)
                    .Select(d => d.system_user_ref!.Value)
                    .ToList();

                var extraUsers = await _context.Users
                    .Where(u => extraUserIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id);

                foreach (var d in extraDoctorInfos
                    .Where(d => d.system_user_ref.HasValue && extraUsers.ContainsKey(d.system_user_ref!.Value)))
                {
                    doctorInfoToUser[d.id] = extraUsers[d.system_user_ref!.Value];
                }
            }

            // ── Specializations (needed by the booking modal) ─────────────
            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .Select(s => new { Id = s.id, Name = s.specialization_name })
                .ToListAsync();

            // ── Calendar events ───────────────────────────────────────────
            var calendarEvents = appointments
                .Where(a => a.date.HasValue)
                .Select(a => new
                {
                    date = a.date!.Value.ToString("yyyy-MM-dd"),
                    name = a.appointment_reason ?? "Appointment",
                    time = SlotToTime(a.appointment_time_slot)
                })
                .ToList();

            ViewBag.PatientInfo = patientInfo;
            ViewBag.PatientId = patientInfo.id;
            ViewBag.Appointments = appointments;
            ViewBag.Specializations = specializations;
            ViewBag.DoctorInfoToUser = doctorInfoToUser;
            ViewBag.MyDoctors = myDoctors;
            ViewBag.Prescriptions = prescriptions;
            ViewBag.CalendarEventsJson = System.Text.Json.JsonSerializer.Serialize(calendarEvents);

            return View(currentUser);
        }

        public ActionResult Book() => View();
        public ActionResult Appointments() => View();
        public ActionResult Prescriptions() => View();
        public ActionResult Notifications() => View();
        public ActionResult History() => View();

        // ─── Helpers ─────────────────────────────────────────────────────────

        private static string SlotToTime(int? slot)
        {
            if (slot == null) return "TBD";
            int minutes = (slot.Value - 1000) * 30;
            int hour = 8 + minutes / 60;
            int min = minutes % 60;
            return new DateTime(2000, 1, 1, hour, min, 0).ToString("hh:mm tt");
        }
    }
}
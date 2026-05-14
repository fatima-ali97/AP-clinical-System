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
                    TotalVisits  = g.Count(a => a.appointment_status == (int)appointment_status.completed),
                    User         = doctorInfoToUser.ContainsKey(g.Key) ? doctorInfoToUser[g.Key] : null
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

            // ── Calendar events ───────────────────────────────────────────
            // appointments is already List<appointment> (in-memory), so SlotToTime resolves fine
            var calendarEvents = appointments
                .Where(a => a.date.HasValue)
                .Select(a => new
                {
                    date = a.date!.Value.ToString("yyyy-MM-dd"),
                    name = a.appointment_reason ?? "Appointment",
                    time = SlotToTime(a.appointment_time_slot)
                })
                .ToList();

            ViewBag.PatientInfo        = patientInfo;
            ViewBag.Appointments       = appointments;
            ViewBag.DoctorInfoToUser   = doctorInfoToUser;
            ViewBag.MyDoctors          = myDoctors;
            ViewBag.Prescriptions      = prescriptions;
            ViewBag.CalendarEventsJson = System.Text.Json.JsonSerializer.Serialize(calendarEvents);

            return View(currentUser);
        }

        // GET /Patient/Book
        public ActionResult Book()
        {
            return View();
        }

        // GET /Patient/Appointments
        public ActionResult Appointments()
        {
            return View();
        }

        // GET /Patient/Prescriptions
        public ActionResult Prescriptions()
        {
            return View();
        }

        // GET /Patient/Notifications
        public ActionResult Notifications()
        {
            return View();
        }

        // GET /Patient/History
        public ActionResult History()
        {
            return View();
        }

        // ─────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────

        /// <summary>
        /// Converts an appointment_time_slot int value to a readable time string.
        /// Slots start at 08:00 (value 1000), each step = 30 minutes.
        /// </summary>
        private static string SlotToTime(int? slot)
        {
            if (slot == null) return "TBD";
            int minutes = (slot.Value - 1000) * 30;
            int hour    = 8 + minutes / 60;
            int min     = minutes % 60;
            return new DateTime(2000, 1, 1, hour, min, 0).ToString("hh:mm tt");
        }
    }
}
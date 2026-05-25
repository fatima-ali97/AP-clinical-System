using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AP_clinical_system.Models.GeneralHelper;

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
        public static List<DoctorObject> GetAllDoctors(AP_Context context)
        {
            var doctors = new List<DoctorObject>();

            var doctorIDs = context.system_users
                .Select(u => new { u.Id, u.user_role, u.inactive })
                .Where(u => u.user_role == (int)user_role.doctor && u.inactive != true)
                .ToList();

            foreach (var doctor in doctorIDs)
            {
                var doctorObject = GetDoctorObjectByID(context, doctor.Id);
                if (doctorObject.Success)
                    doctors.Add(doctorObject);
            }

            return doctors;
        }
        public async Task<ActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            var appointments = await _context.appointments
                .Where(a => a.patient_ref == patientInfo.id && a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

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

            var doctorUsers = await _context.system_users
                .Where(u => doctorUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var doctorInfoToUser = doctorInfos
                .Where(d => d.system_user_ref.HasValue && doctorUsers.ContainsKey(d.system_user_ref!.Value))
                .ToDictionary(d => d.id, d => doctorUsers[d.system_user_ref!.Value]);


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

            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

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

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .Select(s => new { Id = s.id, Name = s.specialization_name })
                .ToListAsync();

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
        public async Task<ActionResult> Appointments()
        {

            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            var appointments = await _context.appointments
                .Where(a => a.patient_ref == patientInfo.id && a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

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

            var doctorUsers = await _context.system_users
                .Where(u => doctorUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var doctorInfoToUser = doctorInfos
                .Where(d => d.system_user_ref.HasValue && doctorUsers.ContainsKey(d.system_user_ref!.Value))
                .ToDictionary(d => d.id, d => doctorUsers[d.system_user_ref!.Value]);

            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

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

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .Select(s => new { Id = s.id, Name = s.specialization_name })
                .ToListAsync();



            ViewBag.PatientInfo = patientInfo;
            ViewBag.PatientId = patientInfo.id;
            ViewBag.Appointments = appointments;
            ViewBag.Specializations = specializations;
            ViewBag.DoctorInfoToUser = doctorInfoToUser;


            return View(currentUser);
        }
        public ActionResult Prescriptions() => View();
        public ActionResult Notifications() => View();
        public ActionResult History() => View();


        [HttpPost]
        public async Task<IActionResult> GetAppointmentInfo(Guid apptID)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            // Get patient_information record
            var patientInfo = await _context.patient_informations
                .FirstOrDefaultAsync(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null) return NotFound();

            // Check BOTH possible ways patient_ref was stored
            var appt = await _context.appointments
                .FirstOrDefaultAsync(a => a.id == apptID
                                       && a.inactive != true
                                       && (a.patient_ref == patientInfo.id
                                           || a.patient_ref == currentUser!.Id));

            if (appt == null) return NotFound();

            var doctor = appt.doctor_ref.HasValue
                ? await _context.doctor_informations.FirstOrDefaultAsync(d => d.id == appt.doctor_ref)
                : null;

            var doctorUser = doctor?.system_user_ref.HasValue == true
                ? await _context.Users.FirstOrDefaultAsync(u => u.Id == doctor.system_user_ref)
                : null;

            var specialization = appt.specialization_ref.HasValue
                ? await _context.doctor_specializations.FirstOrDefaultAsync(s => s.id == appt.specialization_ref)
                : null;

            return Json(new
            {
                appointment_no = appt.appointment_no,
                date = appt.date?.ToString("dd MMM yyyy"),
                time_slot = appt.appointment_time_slot,
                status = appt.appointment_status,
                reason = appt.appointment_reason,
                doctor_name = $"Dr. {Models.GeneralHelper.GetUserFullNameByID(_context, doctorUser.Id)}",
                specialization = specialization?.specialization_name ?? "—"
            });
        }
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
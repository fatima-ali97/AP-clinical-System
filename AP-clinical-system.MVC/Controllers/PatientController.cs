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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
                return Json(new { success = false, error = "First and last name are required." });

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Json(new { success = false, error = "User not found." });

            currentUser.first_name = model.FirstName.Trim();
            currentUser.last_name = model.LastName.Trim();
            currentUser.PhoneNumber = model.Phone?.Trim();
            currentUser.modifiedon = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(currentUser);

            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Json(new { success = false, error = errors });
            }

            return Json(new { success = true });
        }
        private static async Task<Dictionary<Guid, system_user>> BuildUnifiedDoctorMapAsync(
            AP_Context context,
            IEnumerable<Guid> appointmentDoctorRefs)
        {
            var refs = appointmentDoctorRefs.Distinct().ToList();
            if (!refs.Any())
                return new Dictionary<Guid, system_user>();

            var doctorInfos = await context.doctor_informations
                .Where(d => d.inactive != true &&
                            (refs.Contains(d.id) || (d.system_user_ref.HasValue && refs.Contains(d.system_user_ref!.Value))))
                .ToListAsync();

            var userIds = doctorInfos
                .Where(d => d.system_user_ref.HasValue)
                .Select(d => d.system_user_ref!.Value)
                .Distinct()
                .ToList();

            var usersById = await context.system_users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var map = new Dictionary<Guid, system_user>();

            foreach (var d in doctorInfos.Where(d => d.system_user_ref.HasValue && usersById.ContainsKey(d.system_user_ref!.Value)))
            {
                var user = usersById[d.system_user_ref!.Value];
                map[d.id] = user;
                map[d.system_user_ref!.Value] = user;
            }

            foreach (var @ref in refs.Where(r => !map.ContainsKey(r) && usersById.ContainsKey(r)))
            {
                map[@ref] = usersById[@ref];
            }

            return map;
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

            var appointmentDoctorRefs = appointments
                .Where(a => a.doctor_ref.HasValue)
                .Select(a => a.doctor_ref!.Value)
                .Distinct();

            var doctorMap = await BuildUnifiedDoctorMapAsync(_context, appointmentDoctorRefs);

            var myDoctors = appointments
                .Where(a => a.doctor_ref.HasValue)
                .GroupBy(a => a.doctor_ref!.Value)
                .Select(g => new
                {
                    DoctorInfoId = g.Key,
                    TotalVisits = g.Count(a => a.appointment_status == (int)appointment_status.completed),
                    User = doctorMap.TryGetValue(g.Key, out var u) ? u : null
                })
                .Where(d => d.User != null)
                .ToList();

            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

            var prescriptionDoctorRefs = prescriptions
                .Where(p => p.doctor_ref.HasValue)
                .Select(p => p.doctor_ref!.Value)
                .Distinct()
                .Where(r => !doctorMap.ContainsKey(r));

            var extraMap = await BuildUnifiedDoctorMapAsync(_context, prescriptionDoctorRefs);
            foreach (var kvp in extraMap)
                doctorMap.TryAdd(kvp.Key, kvp.Value);

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
            ViewBag.DoctorInfoToUser = doctorMap;
            ViewBag.MyDoctors = myDoctors;
            ViewBag.Prescriptions = prescriptions;
            ViewBag.CalendarEventsJson = System.Text.Json.JsonSerializer.Serialize(calendarEvents);

            return View(currentUser);
        }

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

            var appointmentDoctorRefs = appointments
                .Where(a => a.doctor_ref.HasValue)
                .Select(a => a.doctor_ref!.Value)
                .Distinct();

            var doctorMap = await BuildUnifiedDoctorMapAsync(_context, appointmentDoctorRefs);

            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

            var prescriptionDoctorRefs = prescriptions
                .Where(p => p.doctor_ref.HasValue)
                .Select(p => p.doctor_ref!.Value)
                .Distinct()
                .Where(r => !doctorMap.ContainsKey(r));

            var extraMap = await BuildUnifiedDoctorMapAsync(_context, prescriptionDoctorRefs);
            foreach (var kvp in extraMap)
                doctorMap.TryAdd(kvp.Key, kvp.Value);

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .Select(s => new { Id = s.id, Name = s.specialization_name })
                .ToListAsync();

            ViewBag.PatientInfo = patientInfo;
            ViewBag.PatientId = patientInfo.id;
            ViewBag.Appointments = appointments;
            ViewBag.Specializations = specializations;
            ViewBag.DoctorInfoToUser = doctorMap;

            return View(currentUser);
        }

        public async Task<ActionResult> Prescriptions()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            var prescriptions = await _context.prescriptions
                .Where(p => p.patient_ref == patientInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

            ViewBag.Prescriptions = prescriptions;

            return View(currentUser);
        }

        public ActionResult Notifications() => View();
        public ActionResult History() => View();

        [HttpPost]
        public async Task<IActionResult> GetAppointmentInfo(Guid apptID)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = await _context.patient_informations
                .FirstOrDefaultAsync(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null) return NotFound();

            var appt = await _context.appointments
                .FirstOrDefaultAsync(a => a.id == apptID
                                       && a.inactive != true
                                       && (a.patient_ref == patientInfo.id
                                           || a.patient_ref == currentUser!.Id));

            if (appt == null) return NotFound();

            system_user? doctorUser = null;

            if (appt.doctor_ref.HasValue)
            {
                var doctorInfo = await _context.doctor_informations
                    .FirstOrDefaultAsync(d => d.id == appt.doctor_ref && d.inactive != true);

                if (doctorInfo?.system_user_ref.HasValue == true)
                {
                    doctorUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == doctorInfo.system_user_ref);
                }
                else
                {
                    doctorUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == appt.doctor_ref);
                }
            }

            var specialization = appt.specialization_ref.HasValue
                ? await _context.doctor_specializations
                    .FirstOrDefaultAsync(s => s.id == appt.specialization_ref)
                : null;

            var doctorName = doctorUser != null
                ? $"Dr. {Models.GeneralHelper.GetUserFullNameByID(_context, doctorUser.Id)}"
                : "—";

            return Json(new
            {
                appointment_no = appt.appointment_no,
                date = appt.date?.ToString("dd MMM yyyy"),
                time_slot = appt.appointment_time_slot,
                status = appt.appointment_status,
                reason = appt.appointment_reason,
                doctor_name = doctorName,
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
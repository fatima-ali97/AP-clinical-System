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
    public class DoctorController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;

        public DoctorController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                var notifications = await _context.notifications
                    .Where(n => n.system_user_ref == currentUser.Id && n.inactive != true)
                    .OrderByDescending(n => n.createdon)
                    .ToListAsync();
                ViewBag.Notifications = notifications;
            }
            return View(currentUser);
        }

        public async Task<ActionResult> Notifications()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var notifications = await _context.notifications
                .Where(n => n.system_user_ref == currentUser.Id && n.inactive != true)
                .OrderByDescending(n => n.createdon)
                .ToListAsync();

            ViewBag.Notifications = notifications;
            return View(currentUser);
        }

        // ── Shared helper: build a Guid → system_user map for a set of doctor refs ──
        private static async Task<Dictionary<Guid, system_user>> BuildUnifiedDoctorMapAsync(
            AP_Context context,
            IEnumerable<Guid> appointmentDoctorRefs)
        {
            var refs = appointmentDoctorRefs.Distinct().ToList();
            if (!refs.Any())
                return new Dictionary<Guid, system_user>();

            var doctorInfos = await context.doctor_informations
                .Where(d => d.inactive != true &&
                            (refs.Contains(d.id) ||
                             (d.system_user_ref.HasValue && refs.Contains(d.system_user_ref!.Value))))
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

            foreach (var d in doctorInfos
                         .Where(d => d.system_user_ref.HasValue &&
                                     usersById.ContainsKey(d.system_user_ref!.Value)))
            {
                var user = usersById[d.system_user_ref!.Value];
                map[d.id] = user;
                map[d.system_user_ref!.Value] = user;
            }

            foreach (var r in refs.Where(r => !map.ContainsKey(r) && usersById.ContainsKey(r)))
                map[r] = usersById[r];

            return map;
        }


        // GET: /Doctor/Schedule  (patient's upcoming / confirmed appointments)
        public async Task<IActionResult> Schedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            var appointments = await _context.appointments
                .Where(a => a.patient_ref == patientInfo.id &&
                            a.inactive != true &&
                            (a.appointment_status == 1004 || a.appointment_status == 1005))
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

        public ActionResult Prescriptions()
        {
            return View();
        }

        // GET: /Doctor/MyAppointments
        public async Task<IActionResult> MyAppointments()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            var appointments = await _context.appointments
                .Where(a => a.doctor_ref == doctorInfo.id &&
                            a.inactive != true &&
                            (a.appointment_status == 1004 || a.appointment_status == 1005))
                .OrderByDescending(a => a.date)
                .ToListAsync();

            // patient_ref values on these appointments are patient_information.id (not system_user.Id),
            // so we resolve via patient_informations first, then join to system_users.
            var patientInfoIds = appointments
                .Where(a => a.patient_ref.HasValue)
                .Select(a => a.patient_ref!.Value)
                .Distinct()
                .ToList();

            var patientInfoList = await _context.patient_informations
                .Where(p => patientInfoIds.Contains(p.id) && p.inactive != true)
                .ToListAsync();

            var patientSystemUserIds = patientInfoList
                .Where(p => p.system_user_ref.HasValue)
                .Select(p => p.system_user_ref!.Value)
                .Distinct()
                .ToList();

            var patientSystemUsers = await _context.system_users
                .Where(u => patientSystemUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            // Build patient_information.id → system_user map for the view
            var patientUserMap = new Dictionary<Guid, system_user>();
            foreach (var pi in patientInfoList)
            {
                if (pi.system_user_ref.HasValue &&
                    patientSystemUsers.TryGetValue(pi.system_user_ref.Value, out var su))
                {
                    patientUserMap[pi.id] = su;
                }
            }

            ViewBag.Appointments = appointments;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorInfoToUser = await BuildUnifiedDoctorMapAsync(
                _context,
                appointments.Where(a => a.doctor_ref.HasValue).Select(a => a.doctor_ref!.Value).Distinct());

            return View();
        }

        // GET: /Doctor/Patients
        public async Task<IActionResult> Patients()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var patients = await _context.patient_informations
                .Where(p => p.inactive != true)
                .ToListAsync();

            // Build patient_information.system_user_ref → system_user map for name display
            var systemUserIds = patients
                .Where(p => p.system_user_ref.HasValue)
                .Select(p => p.system_user_ref!.Value)
                .Distinct()
                .ToList();

            var patientUserMap = await _context.system_users
                .Where(u => systemUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            ViewBag.Patients = patients;
            ViewBag.PatientUserMap = patientUserMap;

            return View();
        }

        // GET: /Doctor/PatientDetails/{id}
        public async Task<IActionResult> PatientDetails(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var patientInfo = await _context.patient_informations
                .FirstOrDefaultAsync(p => p.id == id && p.inactive != true);
            if (patientInfo == null) return NotFound();

            var patientSystemUser = patientInfo.system_user_ref.HasValue
                ? await _userManager.FindByIdAsync(patientInfo.system_user_ref.Value.ToString())
                : null;

            var prescriptions = await _context.prescriptions
                .Where(pr => pr.patient_ref == id && pr.inactive != true)
                .OrderByDescending(pr => pr.createdon)
                .ToListAsync();

            var appointments = await _context.appointments
                .Where(a => a.patient_ref == id && a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

            // Setup mapping for appointments table
            var doctorRefs = appointments.Where(a => a.doctor_ref.HasValue).Select(a => a.doctor_ref!.Value).Distinct();
            var doctorUserMap = await BuildUnifiedDoctorMapAsync(_context, doctorRefs);

            var patientUserMap = new Dictionary<Guid, system_user>();
            if (patientSystemUser != null)
            {
                patientUserMap[patientInfo.id] = patientSystemUser;
            }

            var doctorToUserMapping = new Dictionary<Guid, Guid>();
            foreach (var kvp in doctorUserMap)
            {
                doctorToUserMapping[kvp.Key] = kvp.Value.Id;
            }

            // Get current doctor's schedule
            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);

            doctor_schedule docSchedule = null;
            if (doctorInfo != null)
            {
                docSchedule = await _context.doctor_schedules
                    .FirstOrDefaultAsync(ds => ds.doctor_information_ref == doctorInfo.id && ds.inactive != true);
            }

            ViewBag.PatientInfo = patientInfo;
            ViewBag.ProfileUser = patientSystemUser;
            ViewBag.Prescriptions = prescriptions;
            ViewBag.Appointments = appointments;
            ViewBag.DoctorUserMap = doctorUserMap;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorToUserMapping = doctorToUserMapping;
            ViewBag.DoctorSchedule = docSchedule;

            return View();
        }
    }
}
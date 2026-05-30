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
        public ActionResult Index()
        {
            return View();
        }

        public DoctorController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
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



        public async Task<IActionResult> Schedule() //AKA the doctor's appointments that r not completed
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var patientInfo = _context.patient_informations
                .FirstOrDefault(p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return NotFound();

            var appointments = await _context.appointments //&& (a.appointment_status.Equals(appointment_status.completed) || a.appointment_status.Equals(appointment_status.completed))
                .Where(a => a.patient_ref == patientInfo.id && a.inactive != true && (a.appointment_status == 1004 || a.appointment_status == 1005))
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
        public ActionResult Patients()
        {
            return View();
        }


        public ActionResult Prescriptions()
        {
            return View();
        }



    }
}

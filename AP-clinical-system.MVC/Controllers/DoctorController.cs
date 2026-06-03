using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AP_clinical_system.Hubs;
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

        public ActionResult Index()
        {
            return View();
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

        // GET: /Doctor/Schedule
        public async Task<IActionResult> Schedule()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);

            doctor_schedule docSchedule = null;
            if (doctorInfo != null)
            {
                docSchedule = await _context.doctor_schedules
                    .FirstOrDefaultAsync(ds => ds.doctor_information_ref == doctorInfo.id && ds.inactive != true);
            }

            ViewBag.DoctorSchedule = docSchedule;
            return View();
        }

        public async Task<IActionResult> Prescriptions()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            var prescriptions = await _context.prescriptions
                .Where(p => p.doctor_ref == doctorInfo.id && p.inactive != true)
                .OrderByDescending(p => p.createdon)
                .ToListAsync();

            ViewBag.Prescriptions = prescriptions;
            return View();
        }

        // GET: /Doctor/MyAppointments
        public async Task<IActionResult> MyAppointments(string filter = null)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            // Base query for appointments belonging to this doctor
            var appointmentsQuery = _context.appointments
                .Where(a => a.doctor_ref == doctorInfo.id && a.inactive != true);

            // Apply filter based on the requested tab
            var filterLower = (filter ?? "all").ToLowerInvariant();
            if (filterLower == "upcoming")
            {
                // Upcoming: not completed, cancelled, nor no-show
                appointmentsQuery = appointmentsQuery.Where(a =>
                    a.appointment_status != (int)appointment_status.completed &&
                    a.appointment_status != (int)appointment_status.cancelled &&
                    a.appointment_status != (int)appointment_status.no_show);
            }
            else if (filterLower == "completed")
            {
                appointmentsQuery = appointmentsQuery.Where(a => a.appointment_status == (int)appointment_status.completed);
            }
            // else "all" – no additional filtering

            var appointments = await appointmentsQuery
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
            ViewBag.Filter = filterLower; // expose current filter to view

            return View();
        }

        // GET: /Doctor/Patients
        public async Task<IActionResult> Patients()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            // Get patients that have an appointment with this doctor
            var patientIds = await _context.appointments
                .Where(a => a.doctor_ref == doctorInfo.id && a.inactive != true && a.patient_ref.HasValue)
                .Select(a => a.patient_ref!.Value)
                .Distinct()
                .ToListAsync();

            var patients = await _context.patient_informations
                .Where(p => patientIds.Contains(p.id) && p.inactive != true)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSchedule(doctor_schedule model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            var existingSchedule = await _context.doctor_schedules
                .FirstOrDefaultAsync(ds => ds.doctor_information_ref == doctorInfo.id && ds.inactive != true);

            if (existingSchedule != null)
            {
                existingSchedule.monday_start = model.monday_start;
                existingSchedule.monday_end = model.monday_end;
                existingSchedule.tuesday_start = model.tuesday_start;
                existingSchedule.tuesday_end = model.tuesday_end;
                existingSchedule.wednesday_start = model.wednesday_start;
                existingSchedule.wednesday_end = model.wednesday_end;
                existingSchedule.thursday_start = model.thursday_start;
                existingSchedule.thursday_end = model.thursday_end;
                existingSchedule.friday_start = model.friday_start;
                existingSchedule.friday_end = model.friday_end;
                existingSchedule.saturday_start = model.saturday_start;
                existingSchedule.saturday_end = model.saturday_end;
                existingSchedule.sunday_start = model.sunday_start;
                existingSchedule.sunday_end = model.sunday_end;
                existingSchedule.modifiedon = DateTime.UtcNow;
                existingSchedule.modifiedby = currentUser.Id;
            }
            else
            {
                model.id = Guid.NewGuid();
                model.doctor_information_ref = doctorInfo.id;
                model.createdon = DateTime.UtcNow;
                model.createdby = currentUser.Id;
                _context.doctor_schedules.Add(model);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Schedule");
        }

        [HttpPost]
        public async Task<IActionResult> GetAppointmentInfo(Guid apptID)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser!.Id && d.inactive != true);

            if (doctorInfo == null) return NotFound();

            var appt = await _context.appointments
                .FirstOrDefaultAsync(a => a.id == apptID
                                       && a.inactive != true
                                       && a.doctor_ref == doctorInfo.id);

            if (appt == null) return NotFound();

            system_user? patientUser = null;

            if (appt.patient_ref.HasValue)
            {
                var patientInfo = await _context.patient_informations
                    .FirstOrDefaultAsync(p => p.id == appt.patient_ref && p.inactive != true);

                if (patientInfo?.system_user_ref.HasValue == true)
                {
                    patientUser = await _context.Users
                        .FirstOrDefaultAsync(u => u.Id == patientInfo.system_user_ref);
                }
            }

            var specialization = appt.specialization_ref.HasValue
                ? await _context.doctor_specializations
                    .FirstOrDefaultAsync(s => s.id == appt.specialization_ref)
                : null;

            var patientName = patientUser != null
                ? $"{Models.GeneralHelper.GetUserFullNameByID(_context, patientUser.Id)}"
                : "—";

            return Json(new
            {
                appointment_no = appt.appointment_no,
                date = appt.date?.ToString("dd MMM yyyy"),
                time_slot = appt.appointment_time_slot,
                status = appt.appointment_status,
                reason = appt.appointment_reason,
                patient_name = patientName,
                specialization = specialization?.specialization_name ?? "—"
            });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointmentStatus(Guid apptId, int status)
        {
            var appt = await _context.appointments.FindAsync(apptId);
            if (appt == null) return Json(new { success = false, message = "Appointment not found." });

            appt.appointment_status = status;
            appt.modifiedon = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddVisitRecord(visit_record model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Json(new { success = false, message = "Unauthorized" });

            model.id = Guid.NewGuid();
            model.createdon = DateTime.UtcNow;
            model.createdby = currentUser.Id;
            model.inactive = false;

            _context.visit_records.Add(model);

            // Also mark appointment as completed if it was in progress
            var appt = await _context.appointments.FindAsync(model.appointment_ref);
            if (appt != null)
            {
                appt.appointment_status = (int)appointment_status.completed;
                appt.modifiedon = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: /Doctor/PrescriptionDetails/{id}
        [HttpGet]
        public async Task<IActionResult> PrescriptionDetails(Guid id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);
            if (doctorInfo == null) return NotFound();

            var prescription = await _context.prescriptions
                .FirstOrDefaultAsync(p => p.id == id && p.inactive != true && p.doctor_ref == doctorInfo.id);
            if (prescription == null) return NotFound();

            return PartialView("_PrescriptionDetails", prescription);
        }

        // POST: /Doctor/AddPrescription
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPrescription(prescription model)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Json(new { success = false, message = "Unauthorized" });

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == currentUser.Id && d.inactive != true);

            model.id = Guid.NewGuid();
            model.createdon = DateTime.UtcNow;
            model.createdby = currentUser.Id;
            model.doctor_ref = doctorInfo?.id;
            model.inactive = false;

            _context.prescriptions.Add(model);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
        private async Task<IActionResult> UpdateStatus(Guid id, appointment_status status)
        {
            try
            {
                var appt = await _context.appointments.FindAsync(id);
                if (appt == null || appt.inactive == true)
                    return Json(new { success = false, error = "Appointment not found or inactive." });

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, error = "Unauthorized." });

                // verify user is a doctor
                if (!GeneralHelper.VerifyUserType(_context, user.Id, (int)user_role.doctor))
                    return Json(new { success = false, error = "User is not a doctor." });

                // verify doctor owns the appointment
                var doctorInfo = await _context.doctor_informations
                    .FirstOrDefaultAsync(d => d.system_user_ref == user.Id && d.inactive != true);

                if (doctorInfo == null || appt.doctor_ref != doctorInfo.id)
                    return Json(new { success = false, error = "Not authorized to modify this appointment." });

                appt.appointment_status = (int)status;
                appt.modifiedon = DateTime.UtcNow;
                appt.modifiedby = user.Id;

                await _context.SaveChangesAsync();
                var watcher = new AppointmentsWatcher(_context);
                var counts = watcher.CalculateCounts();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.confirmed);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.cancelled);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleAppointment(Guid appointment_id, AP_clinical_system.ViewModels.BookAppointmentViewModel model)
        {
            var appt = await _context.appointments.FindAsync(appointment_id);
            if (appt == null) return NotFound("Appointment not found");

            var doctor = await _userManager.GetUserAsync(User);
            if (doctor == null) return Unauthorized();

            if (!GeneralHelper.VerifyUserType(_context, doctor.Id, (int)user_role.doctor))
                return Json(new { success = false, error = "User is not a doctor." });

            var doctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == doctor.Id && d.inactive != true);

            if (doctorInfo == null || appt.doctor_ref != doctorInfo.id)
                return Json(new { success = false, error = "Not authorized to reschedule this appointment." });

            DateOnly? targetDate = model.date.HasValue ? DateOnly.FromDateTime(model.date.Value) : null;

            appt.date = targetDate;
            appt.appointment_time_slot = model.appointment_time_slot;
            appt.modifiedon = DateTime.UtcNow;
            appt.modifiedby = doctor.Id;

            await _context.SaveChangesAsync();
    

            TempData["Success"] = $"Appointment {appt.appointment_no} rescheduled successfully!";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);

            return RedirectToAction(nameof(MyAppointments));
        }

    }
}
using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AP_clinical_system.ViewModels;

namespace AP_clinical_system.Controllers
{
    public class ReceptionistController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;

        public ReceptionistController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ReceptionistController
        public async Task<ActionResult> Index()
        {
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

            var patientRefs = appointments.Where(a => a.patient_ref.HasValue).Select(a => a.patient_ref!.Value).Distinct().ToList();
            var doctorRefs = appointments.Where(a => a.doctor_ref.HasValue).Select(a => a.doctor_ref!.Value).Distinct().ToList();

            var patientInfos = await _context.patient_informations
                .Where(p => p.inactive != true)
                .ToListAsync();

            var doctorInfos = await _context.doctor_informations
                .Where(d => d.inactive != true)
                .ToListAsync();

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .ToListAsync();

            var userIds = patientInfos.Where(p => p.system_user_ref.HasValue).Select(p => p.system_user_ref!.Value)
                .Union(doctorInfos.Where(d => d.system_user_ref.HasValue).Select(d => d.system_user_ref!.Value))
                .Distinct()
                .ToList();

            var usersById = await _context.system_users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var patientUserMap = new Dictionary<Guid, system_user>();
            foreach (var p in patientInfos.Where(p => p.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(p.system_user_ref!.Value, out var user))
                    patientUserMap[p.id] = user;
            }

            var doctorUserMap = new Dictionary<Guid, system_user>();
            var doctorToUserGuidMap = new Dictionary<Guid, Guid>();
            var doctorSpecializationsMap = new Dictionary<Guid, string>();

            foreach (var d in doctorInfos.Where(d => d.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(d.system_user_ref!.Value, out var user))
                {
                    doctorUserMap[d.id] = user;
                    doctorToUserGuidMap[d.id] = user.Id;
                }

                var spec = await _context.doctor_information_specialization_mtms
                    .FirstOrDefaultAsync(m => m.doctor_information_ref == d.id);
                doctorSpecializationsMap[d.id] = spec != null ? spec.doctor_specialization_ref.ToString() : "";
            }

            ViewBag.Appointments = appointments;
            ViewBag.Patients = patientInfos;
            ViewBag.Doctors = doctorInfos;
            ViewBag.Specializations = specializations;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorUserMap = doctorUserMap;
            ViewBag.DoctorToUserGuidMap = doctorToUserGuidMap;
            ViewBag.DoctorSpecializationsMap = doctorSpecializationsMap;

            return View(new BookAppointmentViewModel());
        }

        // GET: Receptionist/Book
        public async Task<ActionResult> Book()
        {
            // 1. Query all active scheduled appointments for the list matrix grid
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

            // 2. Query ALL Active Master Records required to fill dropdown components safely
            var patientInfos = await _context.patient_informations
                .Where(p => p.inactive != true)
                .ToListAsync();

            var doctorInfos = await _context.doctor_informations
                .Where(d => d.inactive != true)
                .ToListAsync();

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .ToListAsync();

            // 3. Resolve underlying user IDs from the master tables
            var userIds = patientInfos.Where(p => p.system_user_ref.HasValue).Select(p => p.system_user_ref!.Value)
                .Union(doctorInfos.Where(d => d.system_user_ref.HasValue).Select(d => d.system_user_ref!.Value))
                .Distinct()
                .ToList();

            var usersById = await _context.system_users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            // 4. Construct presentation layer lookup mappings
            var patientUserMap = new Dictionary<Guid, system_user>();
            foreach (var p in patientInfos.Where(p => p.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(p.system_user_ref!.Value, out var user))
                    patientUserMap[p.id] = user;
            }

            var doctorUserMap = new Dictionary<Guid, system_user>();
            var doctorToUserGuidMap = new Dictionary<Guid, Guid>();
            foreach (var d in doctorInfos.Where(d => d.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(d.system_user_ref!.Value, out var user))
                {
                    doctorUserMap[d.id] = user;
                    doctorToUserGuidMap[d.id] = user.Id; // Crucial for client-side AJAX availability grids
                }
            }

            // 5. Pack data fields cleanly into layout data bags
            ViewBag.Appointments = appointments;
            ViewBag.Patients = patientInfos;
            ViewBag.Doctors = doctorInfos;
            ViewBag.Specializations = specializations;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorUserMap = doctorUserMap;
            ViewBag.DoctorToUserGuidMap = doctorToUserGuidMap;

            // 6. Return an initialized view model instance to protect against form model binding crashes
            return View(new BookAppointmentViewModel());
        }

        // GET: Receptionist/Calendar
        public async Task<ActionResult> Calendar()
        {
            // 1. Query all active scheduled appointments
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

            // 2. Query ALL Active Master Records required
            var patientInfos = await _context.patient_informations
                .Where(p => p.inactive != true)
                .ToListAsync();

            var doctorInfos = await _context.doctor_informations
                .Where(d => d.inactive != true)
                .ToListAsync();

            var specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .ToListAsync();

            // 3. Resolve underlying user IDs from the master tables
            var userIds = patientInfos.Where(p => p.system_user_ref.HasValue).Select(p => p.system_user_ref!.Value)
                .Union(doctorInfos.Where(d => d.system_user_ref.HasValue).Select(d => d.system_user_ref!.Value))
                .Distinct()
                .ToList();

            var usersById = await _context.system_users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            // 4. Construct presentation layer lookup mappings
            var patientUserMap = new Dictionary<Guid, system_user>();
            foreach (var p in patientInfos.Where(p => p.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(p.system_user_ref!.Value, out var user))
                    patientUserMap[p.id] = user;
            }

            var doctorUserMap = new Dictionary<Guid, system_user>();
            var doctorToUserGuidMap = new Dictionary<Guid, Guid>();
            var doctorSpecializationsMap = new Dictionary<Guid, string>();

            foreach (var d in doctorInfos.Where(d => d.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(d.system_user_ref!.Value, out var user))
                {
                    doctorUserMap[d.id] = user;
                    doctorToUserGuidMap[d.id] = user.Id;
                }

                // Get first specialization for pre-filling the form
                var spec = await _context.doctor_information_specialization_mtms
                    .FirstOrDefaultAsync(m => m.doctor_information_ref == d.id);
                doctorSpecializationsMap[d.id] = spec != null ? spec.doctor_specialization_ref.ToString() : "";
            }

            // 5. Pack data fields cleanly into layout data bags
            ViewBag.Appointments = appointments;
            ViewBag.Patients = patientInfos;
            ViewBag.Doctors = doctorInfos;
            ViewBag.Specializations = specializations;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorUserMap = doctorUserMap;
            ViewBag.DoctorToUserGuidMap = doctorToUserGuidMap;
            ViewBag.DoctorSpecializationsMap = doctorSpecializationsMap;

            return View(new BookAppointmentViewModel());
        }

        // POST: Receptionist/CreateAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAppointment(BookAppointmentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["Error"] = "Failed to book appointment: " + string.Join(" ", errors);
                return RedirectToAction(nameof(Index));
            }

            var receptionist = await _userManager.GetUserAsync(User);
            if (receptionist == null) return Unauthorized();

            // FIX: Convert the ViewModel DateTime? into DateOnly? for comparison
            DateOnly? targetDate = model.date.HasValue ? DateOnly.FromDateTime(model.date.Value) : null;

            // FIX: The AJAX dropdown provides system_user.Id for the doctor, but we need doctor_information.id
            var actualDoctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == model.doctor_ref && d.inactive != true);

            if (actualDoctorInfo == null)
            {
                TempData["Error"] = "Failed to book appointment: Selected doctor information could not be found.";
                return RedirectToAction(nameof(Index));
            }

            bool slotTaken = await _context.appointments.AnyAsync(a =>
                a.doctor_ref == actualDoctorInfo.id &&
                a.date == targetDate && // Fixed type evaluation mismatch
                a.appointment_time_slot == model.appointment_time_slot &&
                a.inactive != true);

            if (slotTaken)
            {
                TempData["Error"] = "Time slot already booked.";
                return RedirectToAction(nameof(Index));
            }

            // Fallback generation helper logic
            var apptNo = Guid.NewGuid().ToString().Substring(0, 8).ToUpper();
            var now = DateTime.UtcNow;

            var appt = new appointment
            {
                id = Guid.NewGuid(),
                appointment_no = apptNo,
                appointment_reason = model.appointment_reason,
                patient_ref = model.patient_ref,
                doctor_ref = actualDoctorInfo.id, // Use resolved ID
                specialization_ref = model.specialization_ref,
                date = targetDate, // FIX: Assign the converted DateOnly? value
                appointment_time_slot = model.appointment_time_slot,
                appointment_status = (int)appointment_status.requested,
                createdon = now,
                createdby = receptionist.Id,
                modifiedon = now,
                modifiedby = receptionist.Id,
                inactive = false
            };

            _context.appointments.Add(appt);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Appointment {apptNo} created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Receptionist/RescheduleAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleAppointment(Guid appointment_id, BookAppointmentViewModel model)
        {
            var appt = await _context.appointments.FindAsync(appointment_id);
            if (appt == null) return NotFound("Appointment not found");

            var receptionist = await _userManager.GetUserAsync(User);
            if (receptionist == null) return Unauthorized();

            DateOnly? targetDate = model.date.HasValue ? DateOnly.FromDateTime(model.date.Value) : null;

            // Optional: update doctor if a replacement is provided
            if (model.doctor_ref.HasValue)
            {
                appt.doctor_ref = model.doctor_ref;
            }

            appt.date = targetDate;
            appt.appointment_time_slot = model.appointment_time_slot;
            appt.modifiedon = DateTime.UtcNow;
            appt.modifiedby = receptionist.Id;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Appointment {appt.appointment_no} rescheduled successfully!";

            // Redirect back to referring page (Calendar or Index/Book)
            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmAppointment(Guid appointment_id) => await UpdateStatus(appointment_id, appointment_status.confirmed);

        [HttpPost]
        public async Task<IActionResult> CheckInAppointment(Guid appointment_id) => await UpdateStatus(appointment_id, appointment_status.checked_in);

        [HttpPost]
        public async Task<IActionResult> NoShowAppointment(Guid appointment_id) => await UpdateStatus(appointment_id, appointment_status.no_show);

        [HttpPost]
        public async Task<IActionResult> CancelAppointment(Guid appointment_id) => await UpdateStatus(appointment_id, appointment_status.cancelled);

        private async Task<IActionResult> UpdateStatus(Guid id, appointment_status status)
        {
            var appt = await _context.appointments.FindAsync(id);
            if (appt == null) return NotFound("Appointment not found");

            var user = await _userManager.GetUserAsync(User);
            appt.appointment_status = (int)status;
            appt.modifiedon = DateTime.UtcNow;
            appt.modifiedby = user?.Id ?? Guid.Empty;

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // GET: Receptionist/Notifications
        public async Task<ActionResult> Notifications()
        {
            return View();
        }
    }
}
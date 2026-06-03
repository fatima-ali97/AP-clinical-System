using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using AP_clinical_system.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AP_clinical_system.ViewModels;
using static AP_clinical_system.Models.GeneralHelper;

namespace AP_clinical_system.Controllers
{
    public class ReceptionistController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;
        private readonly IHubContext<AppointmentsWatcher> _hubContext;

        public ReceptionistController(
            AP_Context context,
            UserManager<system_user> userManager,
            IHubContext<AppointmentsWatcher> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        // GET: Receptionist/Index
        public async Task<ActionResult> Index()
        {
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

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
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

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
            foreach (var d in doctorInfos.Where(d => d.system_user_ref.HasValue))
            {
                if (usersById.TryGetValue(d.system_user_ref!.Value, out var user))
                {
                    doctorUserMap[d.id] = user;
                    doctorToUserGuidMap[d.id] = user.Id;
                }
            }

            ViewBag.Appointments = appointments;
            ViewBag.Patients = patientInfos;
            ViewBag.Doctors = doctorInfos;
            ViewBag.Specializations = specializations;
            ViewBag.PatientUserMap = patientUserMap;
            ViewBag.DoctorUserMap = doctorUserMap;
            ViewBag.DoctorToUserGuidMap = doctorToUserGuidMap;

            return View(new BookAppointmentViewModel());
        }

        // GET: Receptionist/Calendar
        public async Task<ActionResult> Calendar()
        {
            var appointments = await _context.appointments
                .Where(a => a.inactive != true)
                .OrderByDescending(a => a.date)
                .ToListAsync();

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

            // verify user is a receptionist
            if (!GeneralHelper.VerifyUserType(_context, receptionist.Id, (int)user_role.receptionist))
                return Json(new { success = false, error = "User is not a receptionist." });

            DateOnly? targetDate = model.date.HasValue ? DateOnly.FromDateTime(model.date.Value) : null;

            var actualDoctorInfo = await _context.doctor_informations
                .FirstOrDefaultAsync(d => d.system_user_ref == model.doctor_ref && d.inactive != true);

            if (actualDoctorInfo == null)
            {
                TempData["Error"] = "Failed to book appointment: Selected doctor information could not be found.";
                return RedirectToAction(nameof(Index));
            }

            bool slotTaken = await _context.appointments.AnyAsync(a =>
                a.doctor_ref == actualDoctorInfo.id &&
                a.date == targetDate &&
                a.appointment_time_slot == model.appointment_time_slot &&
                a.inactive != true);

            if (slotTaken)
            {
                TempData["Error"] = "Time slot already booked.";
                return RedirectToAction(nameof(Index));
            }

            var apptNo = await GeneralHelper.GetAutonumber(_context, "appointment");
            var now = DateTime.UtcNow;

            var appt = new appointment
            {
                id = Guid.NewGuid(),
                appointment_no = apptNo,
                appointment_reason = model.appointment_reason,
                patient_ref = model.patient_ref,
                doctor_ref = actualDoctorInfo.id,
                specialization_ref = model.specialization_ref,
                date = targetDate,
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

            // broadcast via SignalR
            var watcher = new AppointmentsWatcher(_context);
            var counts = watcher.CalculateCounts();
            await _hubContext.Clients.All.SendAsync("UpdateStatuses", counts);
            await _hubContext.Clients.All.SendAsync("RefreshAppointments");

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

            if (!GeneralHelper.VerifyUserType(_context, receptionist.Id, (int)user_role.receptionist))
                return Json(new { success = false, error = "User is not a receptionist." });

            DateOnly? targetDate = model.date.HasValue ? DateOnly.FromDateTime(model.date.Value) : null;

            if (model.doctor_ref.HasValue)
                appt.doctor_ref = model.doctor_ref;

            appt.date = targetDate;
            appt.appointment_time_slot = model.appointment_time_slot;
            appt.modifiedon = DateTime.UtcNow;
            appt.modifiedby = receptionist.Id;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("RefreshAppointments");

            TempData["Success"] = $"Appointment {appt.appointment_no} rescheduled successfully!";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
                return Redirect(referer);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.confirmed);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.checked_in);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NoShowAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.no_show);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelAppointment(Guid appointment_id)
            => await UpdateStatus(appointment_id, appointment_status.cancelled);

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

                if (!GeneralHelper.VerifyUserType(_context, user.Id, (int)user_role.receptionist))
                    return Json(new { success = false, error = "User is not a receptionist." });

                appt.appointment_status = (int)status;
                appt.modifiedon = DateTime.UtcNow;
                appt.modifiedby = user.Id;

                await _context.SaveChangesAsync();
                var watcher = new AppointmentsWatcher(_context);
                var counts = watcher.CalculateCounts();
                await _hubContext.Clients.All.SendAsync("UpdateStatuses", counts);
                await _hubContext.Clients.All.SendAsync("RefreshAppointments");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Receptionist/Notifications
        public async Task<ActionResult> Notifications()
        {
            return View();
        }
    }
}
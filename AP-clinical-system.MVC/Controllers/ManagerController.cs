using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using AP_clinical_system.Models;
using System.Security.Claims;

namespace AP_clinical_system.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;

        public ManagerController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Manager/Index
        public ActionResult Index() => View();

        // GET: Manager/Doctors
        public ActionResult Doctors() => View();


        [HttpGet]
        public async Task<IActionResult> GetAppointmentsByDayOfWeek()
        {
            var since = DateOnly.FromDateTime(DateTime.Today.AddDays(-90));

            var raw = await _context.appointments
                .Where(a => a.inactive != true && a.date.HasValue && a.date >= since)
                .Select(a => new { a.date })
                .ToListAsync();

            var days = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
            var grouped = raw
                .GroupBy(a => (int)a.date!.Value.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());

            var result = Enumerable.Range(0, 7).Select(i => new
            {
                day = days[i],
                count = grouped.GetValueOrDefault(i, 0)
            }).ToList();

            return Ok(result);
        }

        /// <summary>
        /// Daily totals / completed / cancelled for the last N days.
        /// Powers the trend line chart on the Index dashboard.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAppointmentTrend(int days = 30)
        {
            var since = DateOnly.FromDateTime(DateTime.Today.AddDays(-days + 1));

            var raw = await _context.appointments
                .Where(a => a.inactive != true && a.date.HasValue && a.date >= since)
                .Select(a => new { a.date, a.appointment_status })
                .ToListAsync();

            var grouped = raw
                .GroupBy(a => a.date!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            var result = Enumerable.Range(0, days)
                .Select(i =>
                {
                    var d = DateOnly.FromDateTime(DateTime.Today.AddDays(-days + 1 + i));
                    var list = grouped.GetValueOrDefault(d);
                    return new
                    {
                        date = d.ToString("MMM dd"),
                        total = list?.Count ?? 0,
                        completed = list?.Count(a => a.appointment_status == (int)appointment_status.completed) ?? 0,
                        cancelled = list?.Count(a => a.appointment_status == (int)appointment_status.cancelled) ?? 0
                    };
                })
                .ToList();

            return Ok(result);
        }

        /// <summary>
        /// Today's appointments with resolved names and status labels.
        /// Powers the table on the Index dashboard.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetTodaysAppointments()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var appts = await _context.appointments
                .Where(a => a.inactive != true && a.date == today)
                .OrderBy(a => a.appointment_time_slot)
                .ToListAsync();

            // Doctor info → user name
            var doctorInfoIds = appts
                .Where(a => a.doctor_ref.HasValue).Select(a => a.doctor_ref!.Value)
                .Distinct().ToList();

            var dInfos = await _context.doctor_informations
                .Where(d => doctorInfoIds.Contains(d.id) && d.inactive != true)
                .ToListAsync();

            var dUserIds = dInfos.Where(d => d.system_user_ref.HasValue)
                .Select(d => d.system_user_ref!.Value).ToList();

            var dUsers = await _context.system_users
                .Where(u => dUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var dInfoToUser = dInfos
                .Where(d => d.system_user_ref.HasValue && dUsers.ContainsKey(d.system_user_ref!.Value))
                .ToDictionary(d => d.id, d => dUsers[d.system_user_ref!.Value]);

            // Patient info → user name
            var patientInfoIds = appts
                .Where(a => a.patient_ref.HasValue).Select(a => a.patient_ref!.Value)
                .Distinct().ToList();

            var pInfos = await _context.patient_informations
                .Where(p => patientInfoIds.Contains(p.id) && p.inactive != true)
                .ToListAsync();

            var pUserIds = pInfos.Where(p => p.system_user_ref.HasValue)
                .Select(p => p.system_user_ref!.Value).ToList();

            var pUsers = await _context.system_users
                .Where(u => pUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var pInfoToUser = pInfos
                .Where(p => p.system_user_ref.HasValue && pUsers.ContainsKey(p.system_user_ref!.Value))
                .ToDictionary(p => p.id, p => pUsers[p.system_user_ref!.Value]);

            var statusLabels = new Dictionary<int, string>
            {
                { (int)appointment_status.requested,   "Requested" },
                { (int)appointment_status.confirmed,   "Confirmed" },
                { (int)appointment_status.checked_in,  "Checked In" },
                { (int)appointment_status.in_progress, "In Progress" },
                { (int)appointment_status.completed,   "Completed" },
                { (int)appointment_status.cancelled,   "Cancelled" },
                { (int)appointment_status.no_show,     "No Show" }
            };

            var result = appts.Select(a =>
            {
                var doctorName = "—";
                if (a.doctor_ref.HasValue && dInfoToUser.ContainsKey(a.doctor_ref.Value))
                {
                    var u = dInfoToUser[a.doctor_ref.Value];
                    doctorName = $"Dr. {u.first_name} {u.last_name}".Trim();
                }

                var patientName = "—";
                if (a.patient_ref.HasValue && pInfoToUser.ContainsKey(a.patient_ref.Value))
                {
                    var u = pInfoToUser[a.patient_ref.Value];
                    patientName = $"{u.first_name} {u.last_name}".Trim();
                }

                var statusLabel = a.appointment_status.HasValue
                    && statusLabels.ContainsKey(a.appointment_status.Value)
                        ? statusLabels[a.appointment_status.Value]
                        : "Unknown";

                return new
                {
                    appointment_no = a.appointment_no,
                    doctor = doctorName,
                    patient = patientName,
                    time = SlotToTime(a.appointment_time_slot),
                    status = statusLabel,
                    status_code = a.appointment_status
                };
            }).ToList();

            return Ok(result);
        }

        // ── HELPER ────────────────────────────────────────────────────

        private static string SlotToTime(int? slot)
        {
            if (slot == null) return "TBD";
            int minutes = (slot.Value - 1000) * 30;
            int hour = 8 + minutes / 60;
            int min = minutes % 60;
            return new DateTime(2000, 1, 1, hour, min, 0).ToString("hh:mm tt");
        }



        // GET: Manager/Users
        public async Task<ActionResult> Users()
        {
            var users = await _context.system_users
                .Where(u => u.inactive != true)
                .OrderBy(u => u.user_role)
                .ThenBy(u => u.first_name)
                .Select(u => new Models.UserListViewModel
                {
                    Id = u.Id,
                    FullName = (u.first_name + " " + u.last_name).Trim(),
                    Email = u.Email ?? "—",
                    Phone = u.PhoneNumber ?? "—",
                    CPR = u.cpr ?? "—",
                    UserNo = u.user_no ?? "—",
                    UserRole = u.user_role ?? 0,
                    CreatedOn = u.createdon
                })
                .ToListAsync();

            ViewBag.Specializations = await _context.doctor_specializations
                .Where(s => s.inactive != true)
                .ToListAsync();

            return View(users);
        }

        // GET: Manager/GetUserDetails?userId=...
        [HttpGet]
        public async Task<IActionResult> GetUserDetails(Guid userId)
        {
            var user = await _context.system_users
                .Where(u => u.Id == userId && u.inactive != true)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound();

            var role = (user_role?)user.user_role;
            string? jobTitle = null;
            List<string>? specializations = null;

            // Pull role-specific extra info
            if (role == user_role.doctor)
            {
                var info = await _context.doctor_informations
                    .FirstOrDefaultAsync(d => d.system_user_ref == userId && d.inactive != true);
                jobTitle = info?.job_title;

                if (info != null)
                {
                    var specIds = await _context.doctor_information_specialization_mtms
                        .Where(m => m.doctor_information_ref == info.id)
                        .Select(m => m.doctor_specialization_ref)
                        .ToListAsync();

                    specializations = await _context.doctor_specializations
                        .Where(s => specIds.Contains(s.id) && s.inactive != true)
                        .Select(s => s.specialization_name ?? "")
                        .ToListAsync();
                }
            }
            else if (role == user_role.receptionist)
            {
                var info = await _context.receptionist_informations
                    .FirstOrDefaultAsync(r => r.system_user_ref == userId && r.inactive != true);
                jobTitle = info?.job_title;
            }
            else if (role == user_role.clinic_manager)
            {
                var info = await _context.clinic_manager_informations
                    .FirstOrDefaultAsync(m => m.system_user_ref == userId && m.inactive != true);
                jobTitle = info?.job_title;
            }

            return Json(new
            {
                id = user.Id,
                fullName = $"{user.first_name} {user.last_name}".Trim(),
                email = user.Email ?? "—",
                phone = user.PhoneNumber ?? "—",
                cpr = user.cpr ?? "—",
                userNo = user.user_no ?? "—",
                userRole = user.user_role,
                roleLabel = role?.ToString()?.Replace("_", " ") ?? "Unknown",
                jobTitle = jobTitle ?? "—",
                specializations = specializations ?? new List<string>(),
                createdOn = user.createdon.ToString("dd MMM yyyy")
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddDoctor(string Name, string Email, string Phone, Guid Specialty, string Password)
        {
            var parts = Name.Split(' ');
            var firstName = parts[0];
            var lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";

            var userNo = await GeneralHelper.GetAutonumber(_context, "system_user");

            var user = new system_user
            {
                UserName = Email,
                Email = Email,
                PhoneNumber = Phone,
                first_name = firstName,
                last_name = lastName,
                createdon = DateTime.Now,
                inactive = false,
                user_role = (int)user_role.doctor,
                user_no = userNo
            };

            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                var docInfo = new doctor_information
                {
                    id = Guid.NewGuid(),
                    inactive = false,
                    createdon = DateTime.Now,
                    createdby = user.Id,
                    system_user_ref = user.Id,
                    job_title = "Doctor"
                };
                _context.doctor_informations.Add(docInfo);
                await _context.SaveChangesAsync();

                var docSpec = new doctor_information_specialization_mtm
                {
                    id = Guid.NewGuid(),
                    doctor_information_ref = docInfo.id,
                    doctor_specialization_ref = Specialty
                };
                _context.doctor_information_specialization_mtms.Add(docSpec);
                await _context.SaveChangesAsync();

                await _userManager.AddClaimAsync(user, new Claim("FirstName", firstName));
                await _userManager.AddClaimAsync(user, new Claim("LastName", lastName));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "doctor"));

                TempData["Success"] = "Doctor added successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to add doctor: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        public async Task<IActionResult> AddReceptionist(string Name, string Email, string Phone, string Password)
        {
            var parts = Name.Split(' ');
            var firstName = parts[0];
            var lastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : "";

            var userNo = await GeneralHelper.GetAutonumber(_context, "system_user");

            var user = new system_user
            {
                UserName = Email,
                Email = Email,
                PhoneNumber = Phone,
                first_name = firstName,
                last_name = lastName,
                createdon = DateTime.Now,
                inactive = false,
                user_role = (int)user_role.receptionist,
                user_no = userNo
            };

            var result = await _userManager.CreateAsync(user, Password);
            if (result.Succeeded)
            {
                var recInfo = new receptionist_information
                {
                    id = Guid.NewGuid(),
                    inactive = false,
                    createdon = DateTime.Now,
                    createdby = user.Id,
                    system_user_ref = user.Id,
                    job_title = "Receptionist"
                };
                _context.receptionist_informations.Add(recInfo);
                await _context.SaveChangesAsync();

                await _userManager.AddClaimAsync(user, new Claim("FirstName", firstName));
                await _userManager.AddClaimAsync(user, new Claim("LastName", lastName));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "receptionist"));

                TempData["Success"] = "Receptionist added successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to add receptionist: " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction("Users");
        }
    }


}
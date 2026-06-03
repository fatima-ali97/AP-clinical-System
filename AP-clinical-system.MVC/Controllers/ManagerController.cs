using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AP_clinical_system.Controllers
{
    public class ManagerController : Controller
    {
        private readonly AP_Context _context;

        public ManagerController(AP_Context context)
        {
            _context = context;
        }

        // GET: Manager/Index
        public ActionResult Index() => View();

        // GET: Manager/Doctors
        public ActionResult Doctors() => View();



        // GET: Manager/Appointments
        public ActionResult Appointments() => View();

        // ── REPORTS ────────────────────────────────────────────────────

        // GET: Manager/Reports
        [HttpGet]
        public async Task<IActionResult> Reports(string? startDate, string? endDate)
        {
            startDate ??= DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            endDate ??= DateTime.Now.ToString("yyyy-MM-dd");

            var start = DateOnly.Parse(startDate);
            var end = DateOnly.Parse(endDate);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;

            var appointments = await _context.appointments
                .Where(a => a.inactive != true
                         && a.date.HasValue
                         && a.date >= start
                         && a.date <= end)
                .ToListAsync();

            int total = appointments.Count;
            int completed = appointments.Count(a => a.appointment_status == (int)appointment_status.completed);
            int cancelled = appointments.Count(a => a.appointment_status == (int)appointment_status.cancelled);
            int noShow = appointments.Count(a => a.appointment_status == (int)appointment_status.no_show);
            int requested = appointments.Count(a => a.appointment_status == (int)appointment_status.requested);
            int confirmed = appointments.Count(a => a.appointment_status == (int)appointment_status.confirmed);
            int checkedIn = appointments.Count(a => a.appointment_status == (int)appointment_status.checked_in);
            int inProgress = appointments.Count(a => a.appointment_status == (int)appointment_status.in_progress);

            var appointmentStatistics = new
            {
                summary = new
                {
                    total,
                    completed,
                    cancelled,
                    noShow,
                    requested,
                    confirmed,
                    checkedIn,
                    inProgress
                }
            };

            ViewBag.AppointmentStatistics = JsonSerializer.Serialize(appointmentStatistics);

            // ── 2. Cancellation & No-Show Rates ───────────────────────
            double cancellationRate = total > 0 ? Math.Round((double)cancelled / total * 100, 1) : 0;
            double noShowRate = total > 0 ? Math.Round((double)noShow / total * 100, 1) : 0;
            double combinedRate = total > 0 ? Math.Round((double)(cancelled + noShow) / total * 100, 1) : 0;

            var cancellationAndNoShowRates = new
            {
                overall = new
                {
                    cancellationRate,
                    noShowRate,
                    combinedRate
                }
            };

            ViewBag.CancellationAndNoShowRates = JsonSerializer.Serialize(cancellationAndNoShowRates);

            // ── 3. Appointments by Specialization ─────────────────────
            // appointments.specialization_ref → doctor_specialization.id
            var specializationIds = appointments
                .Where(a => a.specialization_ref.HasValue)
                .Select(a => a.specialization_ref!.Value)
                .Distinct()
                .ToList();

            var specializations = await _context.doctor_specializations
                .Where(s => specializationIds.Contains(s.id) && s.inactive != true)
                .ToListAsync();

            var specializationNameMap = specializations
                .ToDictionary(s => s.id, s => s.specialization_name ?? "Unknown");

            var bySpecialization = appointments
                .Where(a => a.specialization_ref.HasValue)
                .GroupBy(a => a.specialization_ref!.Value)
                .Select(g => new
                {
                    specializationName = specializationNameMap.ContainsKey(g.Key)
                        ? specializationNameMap[g.Key]
                        : "Unknown",
                    totalAppointments = g.Count()
                })
                .OrderByDescending(x => x.totalAppointments)
                .ToList();

            ViewBag.AppointmentsBySpecialization = JsonSerializer.Serialize(
                new { bySpecialization });

            // ── 4. Doctor Workload Distribution ───────────────────────
            // appointments.doctor_ref → doctor_information.id
            // doctor_information.system_user_ref → system_users.Id (for name)
            var appointmentDoctorInfoIds = appointments
                .Where(a => a.doctor_ref.HasValue)
                .Select(a => a.doctor_ref!.Value)
                .Distinct()
                .ToList();

            var allDoctorInfos = await _context.doctor_informations
                .Where(d => d.inactive != true)
                .ToListAsync();

            var activeDoctorInfoIds = allDoctorInfos.Select(d => d.id).ToHashSet();

            var doctorUserIds = allDoctorInfos
                .Where(d => d.system_user_ref.HasValue)
                .Select(d => d.system_user_ref!.Value)
                .ToList();

            var doctorUsers = await _context.system_users
                .Where(u => doctorUserIds.Contains(u.Id) && u.inactive != true)
                .ToDictionaryAsync(u => u.Id);

            // doctor_information.id → display name
            var doctorInfoToName = allDoctorInfos
                .Where(d => d.system_user_ref.HasValue && doctorUsers.ContainsKey(d.system_user_ref!.Value))
                .ToDictionary(
                    d => d.id,
                    d =>
                    {
                        var u = doctorUsers[d.system_user_ref!.Value];
                        return $"Dr. {u.first_name} {u.last_name}".Trim();
                    });

            int totalDoctors = allDoctorInfos.Count;
            int activeDoctors = allDoctorInfos.Count(d =>
                d.system_user_ref.HasValue && doctorUsers.ContainsKey(d.system_user_ref!.Value));

            var doctorWorkloads = appointments
                .Where(a => a.doctor_ref.HasValue)
                .GroupBy(a => a.doctor_ref!.Value)
                .Select(g => new
                {
                    doctorName = doctorInfoToName.ContainsKey(g.Key)
                        ? doctorInfoToName[g.Key]
                        : "Unknown",
                    totalAppointments = g.Count(),
                    completed = g.Count(a => a.appointment_status == (int)appointment_status.completed),
                    cancelled = g.Count(a => a.appointment_status == (int)appointment_status.cancelled)
                })
                .OrderByDescending(x => x.totalAppointments)
                .ToList();

            double avgAppointmentsPerActiveDoctor = activeDoctors > 0
                ? Math.Round((double)total / activeDoctors, 1)
                : 0;

            ViewBag.DoctorWorkloadDistribution = JsonSerializer.Serialize(new
            {
                summary = new
                {
                    totalDoctors,
                    activeDoctors,
                    avgAppointmentsPerActiveDoctor
                },
                doctorWorkloads
            });

            // ── 5. Doctor Leave Summary ────────────────────────────────
            // Leaves that overlap with the selected date range
            var leaves = await _context.doctor_leaves
                .Where(l => l.inactive != true
                         && l.start_date.HasValue
                         && l.end_date.HasValue
                         && l.start_date <= end
                         && l.end_date >= start)
                .ToListAsync();

            var leaveTypeLabels = new Dictionary<int, string>
            {
                { (int)leave_type.annual_leave,             "Annual Leave" },
                { (int)leave_type.sick_leave,               "Sick Leave" },
                { (int)leave_type.unpaid_leave,             "Unpaid Leave" },
                { (int)leave_type.maternity_paternity_leave,"Maternity / Paternity" },
                { (int)leave_type.study_leave,              "Study Leave" },
                { (int)leave_type.other,                    "Other" }
            };

            var leaveTypeBreakdown = leaves
                .Where(l => l.leave_type.HasValue)
                .GroupBy(l => l.leave_type!.Value)
                .Select(g => new
                {
                    leaveType = leaveTypeLabels.ContainsKey(g.Key)
                        ? leaveTypeLabels[g.Key]
                        : "Unknown",
                    count = g.Count()
                })
                .OrderByDescending(x => x.count)
                .ToList();

            int totalLeaveRecords = leaves.Count;
            int doctorsOnLeave = leaves
                .Where(l => l.doctor_information_ref.HasValue)
                .Select(l => l.doctor_information_ref!.Value)
                .Distinct()
                .Count();

            ViewBag.DoctorLeaveSummary = JsonSerializer.Serialize(new
            {
                summary = new
                {
                    totalLeaveRecords,
                    doctorsOnLeave,
                    leaveTypeBreakdown
                }
            });

            return View();
        }

        // ── LIVE DASHBOARD API (Index page) ────────────────────────────

        /// <summary>
        /// Appointment counts grouped by day-of-week for the last 90 days.
        /// Powers the bar chart on the Index dashboard.
        /// </summary>
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
    }


}
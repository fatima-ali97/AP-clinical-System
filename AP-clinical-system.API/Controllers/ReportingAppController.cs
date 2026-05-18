using AP_clinical_system.Models;
using AP_clinical_system.Models.Enums;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AP_clinical_system.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "ClinicManagerOnly")]
    public class ReportingAppController : ControllerBase
    {
        private readonly AP_Context context;

        public ReportingAppController(AP_Context _context)
        {
            context = _context;
        }

      
        [HttpGet]
        public IActionResult AppointmentStatistics([FromBody] JsonElement body)
        {
            var dateValidationError = TryParseDateRange(body, out var startDate, out var endDate);
            if (dateValidationError != null) return dateValidationError;
              
            try
            {
                var appointments = context.appointments
                    .Where(a =>
                        a.inactive != true &&
                        a.date.HasValue &&
                        a.date >= startDate &&
                        a.date <= endDate)
                    .ToList();

                int total        = appointments.Count;
                int completed    = appointments.Count(a => a.appointment_status == (int)appointment_status.completed);
                int cancelled    = appointments.Count(a => a.appointment_status == (int)appointment_status.cancelled);
                int noShow       = appointments.Count(a => a.appointment_status == (int)appointment_status.no_show);

                // Group completed appointments by day for trend data
                var dailyTrend = appointments
                    .Where(a => a.date.HasValue)
                    .GroupBy(a => a.date!.Value)
                    .OrderBy(g => g.Key)
                    .Select(g => new
                    {
                        date              = g.Key.ToString("yyyy-MM-dd"),
                        totalOnDay        = g.Count(),
                        completedOnDay    = g.Count(a => a.appointment_status == (int)appointment_status.completed),
                        cancelledOnDay    = g.Count(a => a.appointment_status == (int)appointment_status.cancelled),
                        noShowOnDay       = g.Count(a => a.appointment_status == (int)appointment_status.no_show),
                    })
                    .ToList();

                return Ok(new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    summary = new
                    {
                        total,
                        completed,
                        cancelled,
                        noShow
                    },
                    dailyTrend
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new 
                { error = "An error occurred while retrieving appointment statistics.", 
                  message = ex.Message 
                });
            }
        }

        [HttpGet]
        public IActionResult CancellationAndNoShowRates([FromBody] JsonElement body)
        {
            var dateValidationError = TryParseDateRange(body, out var startDate, out var endDate);
            if (dateValidationError != null) return dateValidationError;

            try
            {
                var appointments = context.appointments
                    .Where(a =>
                        a.inactive != true &&
                        a.date.HasValue &&
                        a.date >= startDate &&
                        a.date <= endDate)
                    .ToList();

                int total     = appointments.Count;
                int cancelled = appointments.Count(a => a.appointment_status == (int)appointment_status.cancelled);
                int noShow    = appointments.Count(a => a.appointment_status == (int)appointment_status.no_show);

                double cancellationRate = total > 0 ? Math.Round((double)cancelled / total * 100, 2) : 0;
                double noShowRate       = total > 0 ? Math.Round((double)noShow    / total * 100, 2) : 0;
                double combinedRate     = total > 0 ? Math.Round((double)(cancelled + noShow) / total * 100, 2) : 0;

                var doctorIds = appointments
                    .Select(a => a.doctor_ref)
                    .Distinct()
                    .ToList();
                
                var doctorInfos = context.doctor_informations
                    .Where(d => d.inactive != true && doctorIds.Contains(d.id))
                    .ToList();

                var perDoctorBreakdown = doctorInfos.Select(di =>
                {
                    var doctorApps = appointments.Where(a => a.doctor_ref == di.id).ToList();
                    int dTotal     = doctorApps.Count;
                    int dCancelled = doctorApps.Count(a => a.appointment_status == (int)appointment_status.cancelled);
                    int dNoShow    = doctorApps.Count(a => a.appointment_status == (int)appointment_status.no_show);
                    
                    var doctorName = GeneralHelper.GetUserFullNameByID(context, di.system_user_ref.Value);   
                    
                    return new
                    {
                        doctorId          = di.system_user_ref,
                        doctorName,
                        totalAppointments = dTotal,
                        cancelled         = dCancelled,
                        noShow            = dNoShow,
                        cancellationRate  = dTotal > 0 ? Math.Round((double)dCancelled / dTotal * 100, 2) : 0,
                        noShowRate        = dTotal > 0 ? Math.Round((double)dNoShow    / dTotal * 100, 2) : 0,
                    };
                }).OrderByDescending(d => d.cancellationRate).ToList();

                return Ok(new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    overall = new
                    {
                        totalAppointments = total,
                        cancelled,
                        noShow,
                        cancellationRate,
                        noShowRate,
                        combinedRate
                    },
                    perDoctorBreakdown
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while calculating cancellation rates.",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult AppointmentsBySpecialization([FromBody] JsonElement body)
        {
            var dateValidationError = TryParseDateRange(body, out var startDate, out var endDate);
            if (dateValidationError != null) return dateValidationError;

            try
            {
                var appointments = context.appointments
                    .Where(a =>
                        a.inactive != true &&
                        a.date.HasValue &&
                        a.date >= startDate &&
                        a.date <= endDate &&
                        a.specialization_ref.HasValue)
                    .ToList();

                var specializationIds = appointments
                    .Select(a => a.specialization_ref!.Value)
                    .Distinct()
                    .ToList();

                var specializations = context.doctor_specializations
                    .Where(s => specializationIds.Contains(s.id))
                    .ToList();

                var breakdown = specializations.Select(spec =>
                {
                    var specApps    = appointments.Where(a => a.specialization_ref == spec.id).ToList();
                    int specTotal   = specApps.Count;
                    int completed   = specApps.Count(a => a.appointment_status == (int)appointment_status.completed);
                    int cancelled   = specApps.Count(a => a.appointment_status == (int)appointment_status.cancelled);
                    int noShow      = specApps.Count(a => a.appointment_status == (int)appointment_status.no_show);

                    return new
                    {
                        specializationId   = spec.id,
                        specializationName = spec.specialization_name,
                        totalAppointments  = specTotal,
                        completed,
                        cancelled,
                        noShow,
                        completionRate     = specTotal > 0 ? Math.Round((double)completed / specTotal * 100, 2) : 0,
                    };
                }).OrderByDescending(s => s.totalAppointments).ToList();

                int grandTotal = appointments.Count;

                return Ok(new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    totalAppointments = grandTotal,
                    bySpecialization = breakdown
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving appointments by specialization.",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult DoctorWorkloadDistribution([FromBody] JsonElement body)
        {
            var dateValidationError = TryParseDateRange(body, out var startDate, out var endDate);
            if (dateValidationError != null) return dateValidationError;

            try
            {
                var appointments = context.appointments
                    .Where(a =>
                        a.inactive != true &&
                        a.date.HasValue &&
                        a.date >= startDate &&
                        a.date <= endDate)
                    .ToList();

                int grandTotal = appointments.Count;

                var doctorInfos = context.doctor_informations
                    .Where(d => d.inactive != true)
                    .ToList();

                var workload = doctorInfos.Select(di =>
                {
                    var docApps   = appointments.Where(a => a.doctor_ref == di.id).ToList();
                    int total     = docApps.Count;
                    int completed = docApps.Count(a => a.appointment_status == (int)appointment_status.completed);
                    int cancelled = docApps.Count(a => a.appointment_status == (int)appointment_status.cancelled);
                    int noShow    = docApps.Count(a => a.appointment_status == (int)appointment_status.no_show);

                    var doctorName = di.system_user_ref.HasValue ?
                    GeneralHelper.GetUserFullNameByID(context, di.system_user_ref.Value) : "N/A";   

                    return new
                    {
                        doctorId          = di.system_user_ref,
                        doctorName,
                        jobTitle          = di.job_title,
                        totalAppointments = total,
                        completed,
                        cancelled,
                        noShow,
                        workloadShare     = grandTotal > 0 ? Math.Round((double)total / grandTotal * 100, 2) : 0,
                    };
                })
                .OrderByDescending(d => d.totalAppointments)
                .ToList();

                int activeDoctors = workload.Count(d => d.totalAppointments > 0);
                double average    = activeDoctors > 0
                    ? Math.Round((double)grandTotal / activeDoctors, 2)
                    : 0;

                return Ok(new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    summary = new
                    {
                        totalDoctors  = doctorInfos.Count,
                        activeDoctors,
                        totalAppointments  = grandTotal,
                        avgAppointmentsPerActiveDoctor = average
                    },
                    doctorWorkloads = workload
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving workload distribution.",
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public IActionResult DoctorLeaveSummary([FromBody] JsonElement body)
        {
            var dateValidationError = TryParseDateRange(body, out var startDate, out var endDate);
            if (dateValidationError != null) return dateValidationError;

            try
            {
                var leaves = context.doctor_leaves
                    .Where(l =>
                        l.inactive != true &&
                        l.start_date.HasValue &&
                        l.end_date.HasValue &&
                        l.start_date <= endDate &&
                        l.end_date   >= startDate)
                    .ToList();

                var doctorInfos = context.doctor_informations
                    .Where(d => d.inactive != true)
                    .ToList();

                static string LeaveLabel(int? lt) => lt switch
                {
                    (int)leave_type.annual_leave               => "Annual Leave",
                    (int)leave_type.sick_leave                 => "Sick Leave",
                    (int)leave_type.unpaid_leave               => "Unpaid Leave",
                    (int)leave_type.maternity_paternity_leave  => "Maternity/Paternity Leave",
                    (int)leave_type.study_leave                => "Study Leave",
                    (int)leave_type.other                      => "Other",
                    _                                          => "Unknown"
                };

                var perDoctor = doctorInfos.Select(di =>
                {
                    var docLeaves  = leaves.Where(l => l.doctor_information_ref == di.id).ToList();

                    var doctorName = di.system_user_ref.HasValue ?
                     GeneralHelper.GetUserFullNameByID(context, di.system_user_ref.Value) : "N/A";

                    int totalDays = docLeaves.Sum(l =>
                    {
                        var from = l.start_date!.Value < startDate ? startDate : l.start_date!.Value;
                        var to   = l.end_date!.Value   > endDate   ? endDate   : l.end_date!.Value;
                        return (to.DayNumber - from.DayNumber) + 1;
                    });

                    return new
                    {
                        doctorId      = di.system_user_ref,
                        doctorName,
                        jobTitle      = di.job_title,
                        totalLeaves   = docLeaves.Count,
                        totalDaysOff  = totalDays,
                        leaveRecords  = docLeaves.Select(l => new
                        {
                            leaveId    = l.id,
                            leaveType  = LeaveLabel(l.leave_type),
                            startDate  = l.start_date!.Value.ToString("yyyy-MM-dd"),
                            endDate    = l.end_date!.Value.ToString("yyyy-MM-dd"),
                            reason     = l.reason
                        }).ToList()
                    };
                })
                .Where(d => d.totalLeaves > 0)
                .OrderByDescending(d => d.totalDaysOff)
                .ToList();

                var leaveTypeBreakdown = leaves
                    .GroupBy(l => l.leave_type)
                    .Select(g => new
                    {
                        leaveType = LeaveLabel(g.Key),
                        count     = g.Count()
                    })
                    .OrderByDescending(g => g.count)
                    .ToList();

                return Ok(new
                {
                    period = new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") },
                    summary = new
                    {
                        totalLeaveRecords = leaves.Count,
                        doctorsOnLeave    = perDoctor.Count,
                        leaveTypeBreakdown
                    },
                    perDoctorLeave = perDoctor
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while retrieving leave summary.",
                    message = ex.Message
                });
            }
        }
        
        private IActionResult? TryParseDateRange(
            JsonElement body,
            out DateOnly startDate,
            out DateOnly endDate)
        {
            startDate = default;
            endDate   = default;

            if (!body.TryGetProperty("startDate", out var startEl) ||
                !DateOnly.TryParse(startEl.GetString(), out startDate))
                return BadRequest(new { error = "Invalid startDate format. Use yyyy-MM-dd." });

            if (!body.TryGetProperty("endDate", out var endEl) ||
                !DateOnly.TryParse(endEl.GetString(), out endDate))
                return BadRequest(new { error = "Invalid endDate format. Use yyyy-MM-dd." });

            if (endDate < startDate)
                return BadRequest(new { error = "endDate must be on or after startDate." });

            return null;
        }
    }
}
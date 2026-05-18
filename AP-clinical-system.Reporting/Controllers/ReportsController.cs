using AP_clinical_system.Reporting.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Reporting.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApiClient _apiClient;

        public ReportsController(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(
            string? startDate,
            string? endDate)
        {
            startDate ??= DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            endDate ??= DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                ViewBag.StartDate = startDate;
                ViewBag.EndDate = endDate;

                ViewBag.AppointmentStatistics =
                    await _apiClient.GetAppointmentStatisticsAsync(startDate, endDate);

                ViewBag.CancellationAndNoShowRates =
                    await _apiClient.GetCancellationAndNoShowRatesAsync(startDate, endDate);

                ViewBag.AppointmentsBySpecialization =
                    await _apiClient.GetAppointmentsBySpecializationAsync(startDate, endDate);

                ViewBag.DoctorWorkloadDistribution =
                    await _apiClient.GetDoctorWorkloadDistributionAsync(startDate, endDate);

                ViewBag.DoctorLeaveSummary =
                    await _apiClient.GetDoctorLeaveSummaryAsync(startDate, endDate);

                return View();
            }
            catch
            {
                ModelState.AddModelError("", "Could not load reporting data from API.");
                return View();
            }
        }
    }
}

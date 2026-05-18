using AP_clinical_system.Reporting.Dtos;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AP_clinical_system.Reporting.Services
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _contextAccessor;

        public ApiClient(HttpClient http, IHttpContextAccessor contextAccessor)
        {
            _http = http;
            _contextAccessor = contextAccessor;
        }

        public async Task<LoginResponse?> LoginAsync(LoginReq request)
        {
            var response = await _http.PostAsJsonAsync("api/Auth/ReportingAppLogin", request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        private async Task AttachBearerTokenAsync(HttpRequestMessage request)
        {
            var context = _contextAccessor.HttpContext;

            if (context == null)
            {
                return;
            }

            var token = await context.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task<string> SendReportRequestAsync(string endpoint, string startDate, string endDate)
        {
            var body = new
            {
                startDate = startDate,
                endDate = endDate
            };

            var json = JsonSerializer.Serialize(body);

            var request = new HttpRequestMessage(HttpMethod.Get, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            await AttachBearerTokenAsync(request);

            var response = await _http.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public Task<string> GetAppointmentStatisticsAsync(string startDate, string endDate)
        {
            return SendReportRequestAsync(
                "api/ReportingApp/AppointmentStatistics",
                startDate,
                endDate
            );
        }

        public Task<string> GetCancellationAndNoShowRatesAsync(string startDate, string endDate)
        {
            return SendReportRequestAsync(
                "api/ReportingApp/CancellationAndNoShowRates",
                startDate,
                endDate
            );
        }

        public Task<string> GetAppointmentsBySpecializationAsync(string startDate, string endDate)
        {
            return SendReportRequestAsync(
                "api/ReportingApp/AppointmentsBySpecialization",
                startDate,
                endDate
            );
        }

        public Task<string> GetDoctorWorkloadDistributionAsync(string startDate, string endDate)
        {
            return SendReportRequestAsync(
                "api/ReportingApp/DoctorWorkloadDistribution",
                startDate,
                endDate
            );
        }

        public Task<string> GetDoctorLeaveSummaryAsync(string startDate, string endDate)
        {
            return SendReportRequestAsync(
                "api/ReportingApp/DoctorLeaveSummary",
                startDate,
                endDate
            );
        }
    }
}

using AP_clinical_system.Models;
using AP_clinical_system.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace AP_clinical_system.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        ViewBag.StatusCode = statusCode ?? 500;
        ViewBag.Message = statusCode switch
        {
            404 => "The page you're looking for doesn't exist.",
            403 => "You don't have permission to access this page.",
            500 => "Something went wrong on our end.",
            _ => "An unexpected error occurred."
        };
        return View("~/Views/Shared/Error.cshtml");
    }

    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult contact()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Lookup()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Lookup(LookupRequest model)
    {
        if (string.IsNullOrWhiteSpace(model.Cpr) && string.IsNullOrWhiteSpace(model.RecordNo))
        {
            ModelState.AddModelError("", "Please enter either a CPR or a Record Number.");
            return View(model);
        }

        try
        {
            var client = _httpClientFactory.CreateClient("api");

            var payload = new
            {
                cpr = string.IsNullOrWhiteSpace(model.Cpr) ? null : model.Cpr,
                record_no = string.IsNullOrWhiteSpace(model.RecordNo) ? null : model.RecordNo
            };

            var response = await client.PostAsJsonAsync("api/AppointmentLookup/AnonAppLookup", payload);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                string customMessage = null;


                try
                {
                    using (var doc = JsonDocument.Parse(errorBody))
                    {
                        if (doc.RootElement.TryGetProperty("message", out var msgElement))
                        {
                            customMessage = msgElement.GetString();
                        }
                        else if (doc.RootElement.TryGetProperty("error", out var errElement))
                        {
                            customMessage = errElement.GetString();
                        }
                    }
                }
                catch
                {
                    _logger.LogWarning("API error response was not valid JSON. Raw body: {ErrorBody}", errorBody);
                }

                if (!string.IsNullOrEmpty(customMessage))
                {
                    ModelState.AddModelError("", $"Lookup failed:\n {response.StatusCode} : {customMessage}");
                }
                else
                {
                    ModelState.AddModelError("", $"Lookup failed:\n {response.StatusCode} : {response.ReasonPhrase}");
                }

                return View(model);
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            ViewBag.LookupResultJson = resultJson;

            return View(model);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lookup error");
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }
}

using AP_clinical_system.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;

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

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
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
    public async Task<IActionResult> Lookup(string cpr, string record_no)
    {
        if (string.IsNullOrWhiteSpace(cpr) && string.IsNullOrWhiteSpace(record_no))
        {
            ModelState.AddModelError("", "Please enter either a CPR or a Record Number.");
            return View();
        }
        try
        {
            var client = _httpClientFactory.CreateClient("api");

            var payload = new
            {
                cpr = string.IsNullOrWhiteSpace(cpr) ? null : cpr,
                record_no = string.IsNullOrWhiteSpace(record_no) ? null : record_no
            };

            var response = await client.PostAsJsonAsync("AppointmentLookup", payload);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", $"Lookup failed: {response.StatusCode}");
                return View();
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            ViewBag.LookupResultJson = resultJson;

            return View();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View();
        }
    }
}

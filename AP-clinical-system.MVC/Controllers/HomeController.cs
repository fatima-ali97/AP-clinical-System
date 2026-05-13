using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AP_clinical_system.MVC.Models;

namespace AP_clinical_system.MVC.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

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


    
}

using System.Diagnostics;
using AP_clinical_system.Models;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class HomeController : Controller
{
   public IActionResult Index()
{
    if (User.Identity?.IsAuthenticated == true)
        return RedirectToAction("Index", "Dashboard");

    return View();
}

    public IActionResult About() => View();
    public IActionResult Contact() => View();
}
}

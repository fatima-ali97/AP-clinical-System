using System.Diagnostics;
using AP_clinical_system.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    [Authorize]
    public class DashboardController : Controller
{
    
   public IActionResult Index()
    {
        return View();
    }
}
}

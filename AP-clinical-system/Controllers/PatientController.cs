using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class PatientController : Controller
    {
        // GET: PatientController
        public ActionResult Index()
        {
            return View();
        }

        // GET
        public ActionResult Appointments()
        {
            return View();
        }
        // GET
        public ActionResult Prescriptions()
        {
            return View();
        }


        public ActionResult Notifications()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

    }
}

using AP_clinical_system.Models.Entities;
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

        public ActionResult Book()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(appointment app)
        {
            if (ModelState.IsValid)
            {
                ModelState.Clear();
                //yes Model is valid
                //save student details in database
                return View();
            }

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

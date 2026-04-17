using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class ManagerController : Controller
    {

        // GET: PatientController
        public ActionResult Index()
        {
            return View();
        }

        // GET
        public ActionResult Doctors()
        {
            return View();
        }
        // GET
        public ActionResult Reports()
        {
            return View();
        }


        public ActionResult Users()
        {
            return View();
        }

        public ActionResult Appointments()
        {
            return View();
        }

    }
}

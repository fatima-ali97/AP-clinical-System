using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class DoctorController : Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        // GET
        public ActionResult Schedule()
        {
            return View();
        }
        // GET
        public ActionResult Patients()
        {
            return View();
        }


        public ActionResult Prescriptions()
        {
            return View();
        }



    }
}

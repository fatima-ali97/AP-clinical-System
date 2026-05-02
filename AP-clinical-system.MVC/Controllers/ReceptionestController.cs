using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class ReceptionistController : Controller
    {

        // GET: PatientController
        public ActionResult Index()
        {
            return View();
        }

        // GET
        public ActionResult Book()
        {
            return View();
        }
        // GET
        public ActionResult Notifications()
        {
            return View();
        }



    }
}

using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AP_clinical_system.Models.sql_Context;

namespace AP_clinical_system.Controllers
{
    public class DoctorController : Controller
    {
        private readonly AP_Context _context;
        public ActionResult Index()
        {
            return View();
        }

        public DoctorController(AP_Context context)
        {
            _context = context;
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

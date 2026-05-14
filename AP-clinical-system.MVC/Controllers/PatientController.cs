using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AP_clinical_system.Controllers
{
    public class PatientController : Controller
    {
        
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;


        public PatientController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<ActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);


            var patientInfo =  _context.patient_informations.FirstOrDefault
                (p => p.system_user_ref == currentUser!.Id && p.inactive != true);

            if (patientInfo == null)
                return  NotFound();


            // fetch appointments
            var appointments = await _context.appointments
                .Where(a => a.patient_ref == patientInfo.id && a.inactive != true)
                .OrderByDescending(a => a.date).ToListAsync();



            ViewBag.PatientInfo = patientInfo;
            ViewBag.Appointments = appointments;

            return View(currentUser);
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

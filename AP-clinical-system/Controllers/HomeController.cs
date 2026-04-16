using System.Diagnostics;
using AP_clinical_system.Models;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public ActionResult Login(UserProfile objUser)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         using (DB_Entities db = new DB_Entities())
        //         {
        //             var obj = db.UserProfiles.Where(a => a.UserName.Equals(objUser.UserName) && a.Password.Equals(objUser.Password)).FirstOrDefault();
        //             if (obj != null)
        //             {
        //                 Session["UserID"] = obj.UserId.ToString();
        //                 Session["UserName"] = obj.UserName.ToString();
        //                 return RedirectToAction("UserDashBoard");
        //             }
        //         }
        //     }
        //     return View(objUser);
        // }

        // public ActionResult UserDashBoard()
        // {
        //     if (Session["UserID"] != null)
        //     {
        //         return View();
        //     }
        //     else
        //     {
        //         return RedirectToAction("Login");
        //     }
        // }
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

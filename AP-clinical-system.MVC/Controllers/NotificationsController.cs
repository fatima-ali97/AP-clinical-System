using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AP_clinical_system.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AP_Context _context;
        private readonly UserManager<system_user> _userManager;

        public NotificationsController(AP_Context context, UserManager<system_user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /Notifications/GetMyNotifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var result = await NotificationsHelper.GetUserNotificationsAsync(_context, user.Id);
            return result;
        }

        // POST: /Notifications/MarkAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            await NotificationsHelper.MarkNotificationAsReadAsync(_context, notificationId);
            return Json(new { success = true });
        }

        // POST: /Notifications/MarkAllAsRead
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var unread = await _context.notifications
                .Where(n => n.system_user_ref == user.Id && n.is_read != true && n.inactive != true)
                .ToListAsync();

            foreach (var n in unread)
            {
                n.is_read = true;
                n.modifiedon = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}
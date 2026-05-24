using AP_clinical_system.Models;
using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly AP_Context context;

        public NotificationsController(AP_Context _context)
        {
            context = _context;
        }

        public static IActionResult SendNotificationSingle(Guid recepientId, string title, string message)
        {
            var notification = new notification();

            var user = context.system_users.FirstOrDefault(u => u.Id == recepientId && u.inactive != true);

            if (user == null)
            {
              return NotFound("User does not exist or is inactive");
            } else
            {
                CreateNotification(notification, title, message, user.Id);
                return Ok("Success");
            }
        }

        private static void CreateNotification(notification notification, string title, string message, Guid recepientId)
        {
            // Initialize fields
            Guid systemGuid = Guid.Parse("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1");
            var id = Guid.NewGuid();
            var createdon = DateTime.Now;
            var createdby = systemGuid;
            var modifiedon = createdon;
            var modifiedby = createdby;
            var inactive = false;

            // system fields
            notification.id = id;
            notification.createdon = createdon;
            notification.createdby = createdby;
            notification.modifiedon = modifiedon;
            notification.modifiedby = modifiedby;
            notification.inactive = inactive;

            // notification fields
            notification.notification_no = GeneralHelper.GetAutonumber(context, "notification");
            notification.system_user_ref = recepientId;
            notification.title = title;
            notification.message = message;
            notification.is_read = false;
        }
    }
}

using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AP_clinical_system.Models
{
    public static class NotificationsHelper
    {
        public static async Task<IActionResult> SendNotificationSingleAsync(AP_Context context, Guid recepientId, string title, string message)
        {
            var notification = new notification();

            var user = context.system_users.FirstOrDefault(u => u.Id == recepientId && u.inactive != true);

            if (user == null)
            {
                return NotFound("User does not exist or is inactive");
            }
            else
            {
                await CreateNotification(context, user.Id, title, message, notification);
                return Ok("Success");
            }
        }

        public static async Task<IActionResult> SendNotificationMultipleAsync(AP_Context context, List<Guid> recepientIds, string title, string message)
        {
            var notification = new notification();
            var notFoundCount = 0;
            var sentCount = 0;

            foreach (var recepientId in recepientIds)
            {
                var user = context.system_users.FirstOrDefault(u => u.Id == recepientId && u.inactive != true);

                if (user == null)
                {
                    notFoundCount++;
                    continue;
                }
                else
                {
                    await CreateNotification(context, user.Id, title, message, notification);
                    sentCount++;
                }
            }

            return Ok(new
            {
                notFoundCount,
                sentCount
            });
        }

        private async static void CreateNotification(AP_Context context, Guid recepientId, string title, string message, notification notification)
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
            notification.notification_no = await GeneralHelper.GetAutonumber(context, "notification");
            notification.system_user_ref = recepientId;
            notification.title = title;
            notification.message = message;
            notification.is_read = false;
        }

    }
}

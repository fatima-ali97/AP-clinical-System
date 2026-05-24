using AP_clinical_system.Models.Entities;
using AP_clinical_system.Models.sql_Context;
using Microsoft.EntityFrameworkCore;

namespace AP_clinical_system.Models
{
    public static class NotificationsHelper
    {
        public static async Task SendNotificationSingleAsync(AP_Context context, Guid recepientId, string title, string message)
        {
            try
            {
                var user = await context.system_users.FirstOrDefaultAsync(u => u.Id == recepientId && u.inactive != true);

                if (user == null)
                {
                    return;
                }

                var notification = new notification();

                await CreateNotification(context, user.Id, title, message, notification);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification error: {ex.Message}");
            }
        }

        public static async Task SendNotificationMultipleAsync(AP_Context context, List<Guid> recepientIds, string title, string message)
        {
            try
            {
                foreach (var recepientId in recepientIds)
                {
                    try
                    {
                        var user = await context.system_users.FirstOrDefaultAsync(u => u.Id == recepientId && u.inactive != true);

                        if (user == null)
                        {
                            continue;
                        }

                        var notification = new notification();

                        await CreateNotification(context, user.Id, title, message, notification);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed notification for user {recepientId}: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Notification batch error: {ex.Message}");
            }
        }

        private static async Task CreateNotification(AP_Context context, Guid recepientId, string title, string message, notification notification)
        {
            Guid systemGuid = Guid.Parse("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1");

            var createdon = DateTime.UtcNow;

            // System fields
            notification.id = Guid.NewGuid();
            notification.createdon = createdon;
            notification.createdby = systemGuid;
            notification.modifiedon = createdon;
            notification.modifiedby = systemGuid;
            notification.inactive = false;

            // Notification fields
            notification.notification_no = await GeneralHelper.GetAutonumber(context, "notification");
            notification.system_user_ref = recepientId;
            notification.title = title;
            notification.message = message;
            notification.is_read = false;

            context.notifications.Add(notification);
            await context.SaveChangesAsync();
        }
    }
}
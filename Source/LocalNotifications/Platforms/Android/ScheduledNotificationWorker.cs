using System;
using Android.Content;
using AndroidX.Work;

namespace LocalNotifications.Platforms
{
    public class ScheduledNotificationWorker : Worker
    {
        public ScheduledNotificationWorker(Context context, WorkerParameters workerParams)
            : base(context, workerParams)
        {
        }

        public override Result DoWork()
        {
            try
            {
                string notificationRequestJson = InputData.GetString(NotificationConstans.NOTIFICATION_REQUEST);
                bool repeat = InputData.GetBoolean(NotificationConstans.REPEAT, false);

                var notificationService = TryGetDefaultDroidNotificationService();

                if (!string.IsNullOrEmpty(notificationRequestJson))
                {
                    NotificationRequest notificationRequest = notificationRequestJson.JsonToObject<NotificationRequest>();

                    if (repeat)
                    {
                        notificationRequest.LastCalledAt = DateTime.Now;
                        notificationService.SaveScheduledNotification(ApplicationContext, notificationRequest);
                    }
                    else
                    {
                        notificationService.RemoveNotificationFromCache(ApplicationContext, notificationRequest.NotificationId);
                    }

                    notificationService.ShowNotification(ApplicationContext, notificationRequest);
                }

                return Result.InvokeSuccess();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Result.InvokeFailure();
            }
        }

        private static NotificationServiceImpl TryGetDefaultDroidNotificationService() =>
            LocalNotificationCenter.Current is NotificationServiceImpl notificationService
                ? notificationService
                : new NotificationServiceImpl();
    }
}

using System;
using Android.Content;
#if MONOANDROID
using AndroidX.Core.App;
#endif
namespace LocalNotifications.Platform.Droid
{
    [BroadcastReceiver]
    public class ScheduledNotificationReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);
                string notificationDetailsJson = intent.GetStringExtra(NotificationConstans.NOTIFICATION_REQUEST);
                bool repeat = intent.GetBooleanExtra(NotificationConstans.REPEAT, false);


                var notificationService = GetDefaultNotificationService();
                if (string.IsNullOrEmpty(notificationDetailsJson))
                {
                    Android.App.Notification notification = (Android.App.Notification)intent.GetParcelableExtra(NotificationConstans.NOTIFICATION);
                    notification.When = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    int notificationId = intent.GetIntExtra(NotificationConstans.NOTIFICATION_ID,
                        0);

                    if (!repeat)
                        notificationService.RemoveNotificationFromCache(context, notificationId);

                    NotificationService.NotificationReceived(notificationId, string.Empty);
                    notificationManager.Notify(notificationId, notification);
                }
                else
                {
                    NotificationRequest notificationRequest = notificationDetailsJson.JsonToObject<NotificationRequest>();

                    if (repeat)
                    {
                        notificationRequest.LastCalledAt = DateTime.Now;
                        notificationService.SaveScheduledNotification(context, notificationRequest);
                        return;
                    }
                    else
                    {
                        //Console.WriteLine("Remove from cache | " + notificationRequest.Title);
                        notificationService.RemoveNotificationFromCache(context, notificationRequest.NotificationId);
                    }

                    notificationService.ShowNotification(context, notificationRequest);
                }

                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public NotificationService GetDefaultNotificationService() => new NotificationService();
    }
}


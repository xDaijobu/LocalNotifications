using System;
using Android.App;
using Android.Content;

namespace LocalNotifications.Platform.Droid
{
    /// <summary>
    /// ini sudah otomatis ke generate di AndroidManifest.xml
    /// Ref : https://stackoverflow.com/questions/37379170/broadcast-receiver-onreceive-not-being-called-in-xamarin-android
    /// </summary>
    [BroadcastReceiver(Exported = true)]
    [IntentFilter(new[] { "android.intent.action.BOOT_COMPLETED", "android.intent.action.MY_PACKAGE_REPLACED" })]
    public class ScheduledNotificationBootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Console.WriteLine("class ScheduledNotificationBootReceiver");
                string action = intent.Action;
                if (action != null)
                {
                    if (action.Equals(Intent.ActionBootCompleted) || action.Equals(Intent.ActionMyPackageReplaced))
                    {
                        Console.WriteLine("Reschedule");
                        var notificationService = GetDefaultNotificationService();
                        notificationService.RescheduleNotifications(context);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in ScheduledNotificationBootReceiver : " + ex);
            }
        }

        public NotificationService GetDefaultNotificationService() => new NotificationService();
    }
}


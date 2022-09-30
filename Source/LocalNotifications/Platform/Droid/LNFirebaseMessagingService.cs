#if MONOANDROID
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Firebase.Messaging;

namespace LocalNotifications.Platform.Droid
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class LNFirebaseMessagingService : FirebaseMessagingService
    {
        private const string TAG = nameof(FirebaseMessagingService);
        private const string NOTIFICATION_ID_KEY = "notification_id";
        private int notificationId;
        public override void OnNewToken(string p0)
        {
            base.OnNewToken(p0);

            Droid.NotificationService.TokenRefresh(p0);
        }

        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);

            var notification = message.GetNotification();
            if (notification == null)
                ScheduleNotification(message.Data["title"], message.Data["body"], message.Data);
            else
                ScheduleNotification(notification.Title, notification.Body, message.Data);
        }

        public void ScheduleNotification(string title, string message, IDictionary<string, string> data = null)
        {
            notificationId = GetNotificationId(data);

            string payload = string.Empty;
            if (data != null)
            {
                // via editor, type si 'data' itu IDictionary<string,string>
                // tpi klo via runtime, type si 'data' itu bukan IDictionary<string,string> melainkan arraylist
                var newData = new Dictionary<string, string>(data);
                payload = newData.ToJson();
            }

            NotificationCenter.Current.Show(notificationId: notificationId,
                                            title: title,
                                            description: message,
                                            payload: payload,
                                            androidOptions: new AndroidOptions(),
                                            iOSOptions: new iOSOptions());
        }

        protected int GetNotificationId(IDictionary<string, string> keyValuePairs)
        {
            try
            {
                return int.Parse(GetValue(keyValuePairs, NOTIFICATION_ID_KEY));
            }
            catch
            {
                return (int)DateTime.Now.Ticks;
            }
        }

        protected string GetValue(IDictionary<string, string> keyValuePairs, string key)
        {
            try
            {
                return keyValuePairs.FirstOrDefault(x => x.Key == key).Value;
            }
            catch
            {
                Console.WriteLine($"[{TAG}] Key not found");
                return string.Empty;
            }
        }
    }
}


#endif
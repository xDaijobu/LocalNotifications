using System;

namespace LocalNotifications
{
    public delegate void NotificationTappedEventHandler(NotificationEventArgs e);
    public delegate void NotificationReceivedEventHandler(NotificationEventArgs e);

    public class NotificationEventArgs : EventArgs
    {
        public int? NotificationId { get; set; }
        public string Payload { get; set; }
    }

    public delegate void FirebasePushNotificationTokenEventHandler(object source, FirebasePushNotificationTokenEventArgs e);

    public class FirebasePushNotificationTokenEventArgs : EventArgs
    {
        public string Token { get; }
        public FirebasePushNotificationTokenEventArgs(string token)
        {
            Token = token;
        }
    }
}

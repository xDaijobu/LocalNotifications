using System;
#if MONOANDROID || ANDROID
using LocalNotifications.Platform.Droid;
#elif XAMARINIOS || IOS
using LocalNotifications.Platform.iOS;
#endif

namespace LocalNotifications
{
    public static class NotificationCenter
    {
        private static INotificationService _current
        {
            get
            {
#if NETSTANDARD2_0
                return null;
#else
                return new NotificationService();
#endif
            }
        }

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current
        {
            get => _current ?? throw new InvalidOperationException("Plugin not found");
        }
    }
}

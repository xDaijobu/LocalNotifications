using System;
using System.Threading;

#if ANDROID || IOS
using LocalNotifications.Platforms;
#endif

namespace LocalNotifications
{
    public partial class LocalNotificationCenter
    {
        public static readonly Lazy<INotificationService?> implementation = new(CreateNotificationService, LazyThreadSafetyMode.PublicationOnly);

        private static INotificationService? CreateNotificationService() =>
#if ANDROID || IOS || WINDOWS
            new NotificationServiceImpl();
#else
    null;
#endif

        /// <summary>
        /// Platform specific INotificationService.
        /// </summary>
        public static INotificationService Current => implementation.Value;
    }
}

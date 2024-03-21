using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalNotifications
{
    // TODO Cris: ubah jdi Task ( biar bisa pakai await .. )
    public interface INotificationService
    {
        void RegisterForPushNotifications();
        void UnRegisterForPushNotifications();

        event NotificationTappedEventHandler OnNotificationTapped;
        event NotificationReceivedEventHandler OnNotificationReceived;
        event FirebasePushNotificationTokenEventHandler OnTokenRefresh;
        
        List<NotificationRequest> GetPendingNotificationRequests();
        void Cancel(int notificationId);
        void CancelAll();

        /// <summary>
        /// Show notification...........
        /// </summary>
        void Show(int notificationId, string title, string description, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
        /// <summary>
        /// Show a notification on an hourly interval ( Once per hour )
        /// </summary>
        void ShowHourly(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
        /// <summary>
        /// Show a notification on a daily interval ( Once per day )
        /// </summary>
        void ShowDaily(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
        /// <summary>
        /// Show a notification on a weekly interval ( Once every week )
        /// </summary>
        void ShowWeekly(int notificationId, string title, string description, Day weekDay, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
        /// <summary>
        /// Schedules a notification to be shown at the specified time
        /// </summary>
        void Schedule(int notificationId, string title, string description, DateTime dateTime, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);

        Task<string> GetTokenAsync();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> RequestNotificationPermission(NotificationPermission permission = null);

        Task<bool> IsNotificationsEnabled();
    }
}

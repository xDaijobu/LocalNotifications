using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LocalNotifications.UnitTests.Mocks
{
    public class MockNotificationService : INotificationService
    {
        public MockNotificationService()
        {
        }

        public event NotificationTappedEventHandler OnNotificationTapped;
        public event NotificationReceivedEventHandler OnNotificationReceived;
        public event FirebasePushNotificationTokenEventHandler OnTokenRefresh;

        public void RaiseNotificationTapped(NotificationEventArgs args)
        {
            OnNotificationTapped?.Invoke(args);
        }

        public void RaiseNotificationReceived(NotificationEventArgs args)
        {
            OnNotificationReceived?.Invoke(args);
        }

        public void RaiseTokenRefresh(FirebasePushNotificationTokenEventArgs args)
        {
            OnTokenRefresh?.Invoke(this, args);
        }

        public void Cancel(int notificationId)
        {
            throw new PlatformNotSupportedException();
        }

        public void CancelAll()
        {
            throw new PlatformNotSupportedException();
        }

        public List<NotificationRequest> GetPendingNotificationRequests()
        {
            throw new PlatformNotSupportedException();
        }

        public Task<string> GetTokenAsync()
        {
            throw new PlatformNotSupportedException();
        }

        public void RegisterForPushNotifications()
        {
            throw new PlatformNotSupportedException();
        }

        public void Schedule(int notificationId, string title, string description, DateTime dateTime, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void Show(int notificationId, string title, string description, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void ShowDaily(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void ShowHourly(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void ShowWeekly(int notificationId, string title, string description, Day weekDay, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            throw new PlatformNotSupportedException();
        }

        public void UnRegisterForPushNotifications()
        {
            throw new PlatformNotSupportedException();
        }
    }
}

using System;
using System.Globalization;
using Foundation;
using UIKit;
using UserNotifications;

namespace LocalNotifications.Platforms
{
    public class LocalNotificationDelegate : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            try
            {
                if (response is null)
                    return;

                if (!response.IsDefaultAction)
                    return;

                if (response.Notification.Request.Content.Badge != null)
                {
                    UIApplication.SharedApplication.InvokeOnMainThread(() =>
                    {
                        nint appBadges = UIApplication.SharedApplication.ApplicationIconBadgeNumber - response.Notification.Request.Content.Badge.Int32Value;
                        UIApplication.SharedApplication.ApplicationIconBadgeNumber = appBadges;
                    });
                }

                NSDictionary dictionary = response.Notification.Request.Content.UserInfo;
                iOSNotification iOSNotification = GetiOSNotification(dictionary);

                if (iOSNotification != null)
                    NotificationServiceImpl.OnPlatformNotificationTapped(iOSNotification.Payload, iOSNotification.NotificationId);

                completionHandler?.Invoke();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DidReceiveNotificationResponse : " + ex.Message);
            }
        }

        public override void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
        {
            try
            {
                //default value
                UNNotificationPresentationOptions presentationOptions = UNNotificationPresentationOptions.None;
                UNNotificationContent notificationContent = notification?.Request.Content;

                if (notificationContent != null)
                {
                    NSDictionary dictionary = notificationContent.UserInfo;

                    iOSNotification iOSNotification = GetiOSNotification(dictionary);
                    NotificationServiceImpl.OnPlatformNotificationReceived(iOSNotification.Payload, iOSNotification.NotificationId);

                    NSNumber _presentAlert = (NSNumber)notification.Request.Content.UserInfo[NotificationServiceImpl.PRESENT_ALERT];
                    NSNumber _playSound = (NSNumber)notification.Request.Content.UserInfo[NotificationServiceImpl.PLAY_SOUND];
                    NSNumber _showBadge = (NSNumber)notification.Request.Content.UserInfo[NotificationServiceImpl.SHOW_BADGE];

                    if (_presentAlert != null && _presentAlert.BoolValue)
                        presentationOptions |= UNNotificationPresentationOptions.Alert;
                    if (_playSound != null && _playSound.BoolValue)
                        presentationOptions |= UNNotificationPresentationOptions.Sound;
                    if (_showBadge != null && _showBadge.BoolValue)
                        presentationOptions |= UNNotificationPresentationOptions.Badge;

                    // HACK Cris ~
                    if (iOSNotification.IsFromFirebase)
                        presentationOptions = UNNotificationPresentationOptions.Alert;
                }
                    
                completionHandler?.Invoke(presentationOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in WillPresentNotification : " + ex.Message);
            }
        }

        private iOSNotification GetiOSNotification(NSDictionary dictionary)
        {
            string payload = dictionary[NotificationServiceImpl.PAYLOAD]?.ToString();
            int? notificationId = (dictionary[NotificationServiceImpl.NOTIFICATION_ID] as NSNumber)?.Int32Value;
            bool isFromFirebase = false;
            if (string.IsNullOrEmpty(payload) && !notificationId.HasValue)
            {
                var error = new NSError();
                var jsonData = NSJsonSerialization.Serialize(dictionary, NSJsonWritingOptions.PrettyPrinted, out error);
                var jsonText = new NSString(jsonData, NSStringEncoding.UTF8);

                // Re-Get Payload & NotificatioinId from Firebase
                payload = jsonText;

                string notifId = dictionary[NotificationServiceImpl.NOTIFICATION_ID_FIREBASE]?.ToString();
                if (!string.IsNullOrEmpty(notifId))
                {
                    notificationId = new NSNumber(int.Parse(notifId)).Int32Value;
                    isFromFirebase = true;
                }
            }


            return new iOSNotification(notificationId, payload, isFromFirebase);
        }

        private class iOSNotification
        {
            public int? NotificationId { get; }
            public string Payload { get; }
            public bool IsFromFirebase { get; }

            public iOSNotification(int? notificationId, string payload, bool isFromFirebase)
            {
                NotificationId = notificationId;
                Payload = payload;
                IsFromFirebase = isFromFirebase;
            }
        }
    }
}


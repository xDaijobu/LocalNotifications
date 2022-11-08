using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Firebase.CloudMessaging;
using Firebase.Core;
using Foundation;
using UIKit;
using UserNotifications;

namespace LocalNotifications.Platform.iOS
{
    public class NotificationService : UNUserNotificationCenterDelegate, INotificationService, IUNUserNotificationCenterDelegate
    {
        public static readonly NSString TITLE = new NSString("Title");
        public static readonly NSString BODY = new NSString("Body");
        public static readonly NSString NOTIFICATION_ID = new NSString("NoticationId");
        public static readonly NSString NOTIFICATION_ID_FIREBASE = new NSString("notification_id");
        public static readonly NSString PAYLOAD = new NSString("Payload");
        public static readonly NSString PAYLOAD_FIREBASE = new NSString("data");
        public static readonly NSString PRESENT_ALERT = new NSString("PresentAlert");
        public static readonly NSString PLAY_SOUND = new NSString("PlaySound");
        public static readonly NSString SOUND = new NSString("Sound");
        public static readonly NSString SHOW_BADGE = new NSString("ShowBadge");
        public static readonly NSString BADGE_NUMBER = new NSString("BadgeNumber");
        public static readonly NSString NOTIFICATION_LAUNCHED_APP = new NSString("NotificationLaunchedApp");
        public static readonly NSString NOTIFICATION_REQUEST = new NSString("NotificationRequest");

        private static FirebaseMessagingDelegate FBMessagingDelegate = new FirebaseMessagingDelegate
        (
            token =>
            {
                onTokenRefresh?.Invoke(NotificationCenter.Current, new FirebasePushNotificationTokenEventArgs(token));
            }
        );

        public NotificationService()
        {
        }

        public static void Initialize(NSDictionary options, bool isFirebase, bool autoRegistration = true)
        {
            if (isFirebase)
            {
                if (App.DefaultInstance == null)
                {
                    App.Configure();
                }

                Messaging.SharedInstance.AutoInitEnabled = autoRegistration;
                Messaging.SharedInstance.Delegate = FBMessagingDelegate;
      
                if (autoRegistration)
                    NotificationCenter.Current.RegisterForPushNotifications();
            }
            else
            {
                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();
            }
        }

        private static NotificationTappedEventHandler onNotificationTapped;
        public event NotificationTappedEventHandler OnNotificationTapped
        {
            add => onNotificationTapped += value;
            remove => onNotificationTapped -= value;
        }

        public static void OnPlatformNotificationTapped(string payload, int? notificationId)
        {
            NotificationEventArgs e = new NotificationEventArgs()
            {
                NotificationId = notificationId,
                Payload = payload
            };

            onNotificationTapped?.Invoke(e);
        }

        private static NotificationReceivedEventHandler onNotificationReceived;
        public event NotificationReceivedEventHandler OnNotificationReceived
        {
            add => onNotificationReceived += value;
            remove => onNotificationReceived -= value;
        }

        public static void OnPlatformNotificationReceived(string payload, int? notificationId)
        {
            NotificationEventArgs e = new NotificationEventArgs()
            {
                NotificationId = notificationId,
                Payload = payload
            };

            onNotificationReceived?.Invoke(e);
        }

        private static FirebasePushNotificationTokenEventHandler onTokenRefresh;
        public event FirebasePushNotificationTokenEventHandler OnTokenRefresh
        {
            add => onTokenRefresh += value;
            remove => onTokenRefresh -= value;
        }

        public void RegisterForPushNotifications()
        {
            // Register your app for remote notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // For iOS 10 display notification (sent via APNS)
                UNUserNotificationCenter.Current.Delegate = new LocalNotificationDelegate();

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;

                UNUserNotificationCenter.Current.RequestAuthorization(authOptions, async (granted, error) =>
                {
                    Console.WriteLine(granted);
                    await Task.Delay(500);
                });
            }
            else
            {
                // iOS 9 or before
                var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
                var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            UIApplication.SharedApplication.RegisterForRemoteNotifications();
        }

        public void UnRegisterForPushNotifications()
        {

        }

        public Task<string> GetTokenAsync()
            => Task.FromResult(Messaging.SharedInstance.FcmToken);

        public static Task RequestPermissions()
            => RequestPermissionsAsync();

        public static async Task<bool> RequestPermissionsAsync()
        {
            try
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0) == false)
                    return true;

                UNNotificationSettings settings = await UNUserNotificationCenter.Current.GetNotificationSettingsAsync().ConfigureAwait(false);

                if (settings.AlertSetting == UNNotificationSetting.Enabled)
                    return true;

                var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
                var (alertsAllowed, error) = await UNUserNotificationCenter.Current.RequestAuthorizationAsync(authOptions).ConfigureAwait(false);

                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in RequestPermissionsAsync: " + ex.Message);
                return false;
            }
        }

        private UNMutableNotificationContent BuildStandardNotificationContent(NotificationRequest notificationRequest)
        {
            UNMutableNotificationContent content = new UNMutableNotificationContent();
            content.Title = notificationRequest.Title;
            content.Body = notificationRequest.Description;

            iOSOptions iOSOptions = notificationRequest.iOS;
            if (!string.IsNullOrEmpty(iOSOptions.Sound))
                content.Sound = UNNotificationSound.GetSound(iOSOptions.Sound);

            if (notificationRequest.iOS.BadgeNumber.HasValue)
                content.Badge = new NSNumber(iOSOptions.BadgeNumber.Value);

            content.Sound ??= UNNotificationSound.Default;

            content.UserInfo = BuildUserDictionary(notificationRequest);
            return content;
        }

        private NSDictionary BuildUserDictionary(NotificationRequest notificationRequest)
        {
            string notificationRequestJson = notificationRequest.ToJson();

            NSMutableDictionary userDictionary = new NSMutableDictionary
            {
                { NOTIFICATION_ID, new NSNumber(notificationRequest.NotificationId) },
                { TITLE, new NSString(notificationRequest.Title) },
                { PRESENT_ALERT, new NSNumber(notificationRequest.iOS.PresentAlert) },
                { PLAY_SOUND, new NSNumber(notificationRequest.iOS.PlaySound) },
                { SHOW_BADGE, new NSNumber(notificationRequest.iOS.ShowBadge) },
                { PAYLOAD, new NSString(notificationRequest.Payload) },
                { NOTIFICATION_REQUEST, new NSString(notificationRequestJson) }
            };

            return userDictionary;
        }

        public async void Show(int notificationId, string title, string description, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            UNCalendarNotificationTrigger trigger = null;
            try
            {
                if (!await RequestPermissionsAsync().ConfigureAwait(false))
                    return;

                iOSOptions ??= new iOSOptions();

                NotificationRequest notificationRequest = GetNotificationRequest(notificationId, title, description, payload, iOSOptions);
                UNMutableNotificationContent content = BuildStandardNotificationContent(notificationRequest);
                NSDateComponents notifyTime = GetNsDateComponentsFromDateTime(notificationRequest.NotifyTime);
                trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, false);

                await AddNotificationRequestAsync(notificationId.ToString(), content, trigger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Show: " + ex.Message);
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        public async void ShowHourly(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            await Repeat(notificationId, title, description, Day.None, time, NotificationRepeat.Hourly, payload, iOSOptions).ConfigureAwait(false);
        }

        public async void ShowDaily(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            await Repeat(notificationId, title, description, Day.None, time, NotificationRepeat.Daily, payload, iOSOptions).ConfigureAwait(false);
        }

        public async void ShowWeekly(int notificationId, string title, string description, Day weekDay, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            await Repeat(notificationId, title, description, weekDay, time, NotificationRepeat.Weekly, payload, iOSOptions).ConfigureAwait(false);
        }

        public async void Schedule(int notificationId, string title, string description, DateTime dateTime, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            UNCalendarNotificationTrigger trigger = null;
            try
            {
                if (!await RequestPermissionsAsync().ConfigureAwait(false))
                    return;

                if (iOSOptions == null)
                    iOSOptions = new iOSOptions();

                //To Epoch timestamp
                var notifyTimeSinceEpoch = dateTime.GetMilliSecondsSinceEpoch();

                NotificationRequest notificationRequest = GetNotificationRequest(notificationId, title, description, payload, iOSOptions);
                notificationRequest.NotifyTime = dateTime;
                notificationRequest.NotifyTimeSinceEpoch = notifyTimeSinceEpoch;

                UNMutableNotificationContent content = BuildStandardNotificationContent(notificationRequest);
                NSDateComponents notifyTime = GetNsDateComponentsFromDateTime((DateTime?)dateTime);
                trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, false);

                await AddNotificationRequestAsync(notificationId.ToString(), content, trigger).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Schedule: " + ex.Message);
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        private async Task Repeat(int notificationId, string title, string description, Day weekDay, Time time, NotificationRepeat repeatInterval, string payload, iOSOptions iOSOptions)
        {
            UNCalendarNotificationTrigger trigger = null;
            try
            {
                iOSOptions ??= new iOSOptions();

                NotificationRequest notificationRequest = new NotificationRequest()
                {
                    NotificationId = notificationId,
                    Title = title,
                    Description = description,
                    CalledAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    RepeatInterval = repeatInterval,
                    WeekDay = weekDay,
                    Time = time,
                    Repeats = true,
                    Payload = payload,
                    iOS = iOSOptions,
                    Android = null
                };

                UNMutableNotificationContent content = BuildStandardNotificationContent(notificationRequest);
                NSDateComponents notifyTime = GetNsDateComponents(notificationRequest.WeekDay, notificationRequest.Time);

                trigger = UNCalendarNotificationTrigger.CreateTrigger(notifyTime, notificationRequest.Repeats);
                //Console.WriteLine("NextTriggerDate : " + trigger.NextTriggerDate);
                await AddNotificationRequestAsync(notificationId.ToString(), content, trigger);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Repeat[iOS]: " + ex.Message);
            }
            finally
            {
                trigger?.Dispose();
            }
        }

        public async Task AddNotificationRequestAsync(string notificationId, UNNotificationContent notificationContent, UNCalendarNotificationTrigger trigger)
        {
            var notificationRequest = UNNotificationRequest.FromIdentifier(notificationId, notificationContent, trigger);

            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(notificationRequest).ConfigureAwait(false);
        }

        public async Task AddNotificationRequestAsync(string notificationId, UNNotificationContent notificationContent, UNTimeIntervalNotificationTrigger trigger)
        {
            var notificationRequest = UNNotificationRequest.FromIdentifier(notificationId, notificationContent, trigger);

            await UNUserNotificationCenter.Current.AddNotificationRequestAsync(notificationRequest).ConfigureAwait(false);
        }

        public void Cancel(int notificationId)
        {
            try
            {
                string[] itemList = new[]
                {
                    notificationId.ToString(CultureInfo.CurrentCulture)
                };

                UNUserNotificationCenter.Current.RemovePendingNotificationRequests(itemList);
                UNUserNotificationCenter.Current.RemoveDeliveredNotifications(itemList);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in Cancel: " + ex.Message);
            }
        }

        public void CancelAll()
        {
            try
            {
                UNUserNotificationCenter.Current.RemoveAllPendingNotificationRequests();
                UNUserNotificationCenter.Current.RemoveAllDeliveredNotifications();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in CancelAll: " + ex.Message);
            }

        }

        public List<NotificationRequest> GetPendingNotificationRequests()
        {
            List<NotificationRequest> scheduledNotifications = new List<NotificationRequest>();

            UILocalNotification[] notifications = UIApplication.SharedApplication.ScheduledLocalNotifications;

            foreach (var notification in notifications)
            {
                if (notification != null)
                {
                    var dictionary = GetParameters(notification.UserInfo);

                    string notificationRequestJson = (string)dictionary[NOTIFICATION_REQUEST];
                    if (!string.IsNullOrEmpty(notificationRequestJson))
                    {
                        NotificationRequest notificationRequest = notificationRequestJson.JsonToObject<NotificationRequest>();
                        scheduledNotifications.Add(notificationRequest);
                    }
                    else
                    {
                        scheduledNotifications.Add(new NotificationRequest()
                        {
                            NotificationId = int.Parse((string)dictionary[NOTIFICATION_ID]),
                            Title = (string)dictionary[TITLE],
                            Description = notification.AlertBody,
                            Payload = (string)dictionary[PAYLOAD],
                        });
                    }
                }
            }

            return scheduledNotifications;
        }

#region Others
        private static IDictionary<string, object> GetParameters(NSDictionary dictionary)
        {
            var parameters = new Dictionary<string, object>();

            foreach (var data in dictionary)
                parameters.Add($"{data.Key}", $"{data.Value}");

            return parameters;
        }

        private NSDateComponents GetNsDateComponentsFromDateTime(DateTime? notifyTime)
        {
            var dateTime = notifyTime ?? DateTime.Now.AddSeconds(1);

            return new NSDateComponents()
            {
                Day = dateTime.Day,
                Hour = dateTime.Hour,
                Minute = dateTime.Minute,
                Second = dateTime.Second
            };
        }

        private NSDateComponents GetNsDateComponents(Day weekDay, Time time)
        {
            var notifyTime = new NSDateComponents();
            notifyTime.Hour = time.Hour;
            notifyTime.Minute = time.Minute;
            notifyTime.Second = time.Second;

            if (weekDay != Day.None)
                notifyTime.Weekday = (int)weekDay;

            return notifyTime;
        }

        private NotificationRequest GetNotificationRequest(int notificationId, string title, string description, string payload, iOSOptions iOSOptions)
        {
            return new NotificationRequest()
            {
                NotificationId = notificationId,
                Title = title,
                Description = description,
                Payload = payload,
                iOS = iOSOptions,
            };
        }
#endregion
    }
}
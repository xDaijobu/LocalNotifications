using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Icu.Util;
using Android.Media;
using Android.OS;
using AndroidX.Core.App;
using Firebase.Messaging;
using Android.Gms.Extensions;
using Xamarin.Essentials;

namespace LocalNotifications.Platform.Droid
{
    public class NotificationService : INotificationService
    {
        /*
         * Read Me
         * https://developer.android.com/reference/android/app/PendingIntent#FLAG_IMMUTABLE
         * Up until Build.VERSION_CODES.R, PendingIntents are assumed to be mutable by default, unless FLAG_IMMUTABLE is set. 
         * Starting with Build.VERSION_CODES.S, it will be required to explicitly specify the mutability of PendingIntents on creation with either (@link #FLAG_IMMUTABLE} or FLAG_MUTABLE. 
         * It is strongly recommended to use FLAG_IMMUTABLE when creating a PendingIntent. 
         * FLAG_MUTABLE should only be used when some functionality relies on modifying the underlying intent, e.g. any PendingIntent that needs to be used with inline reply or bubbles.
         */
        private static PendingIntentFlags pendingIntentFlags = ((int)Build.VERSION.SdkInt >= 31)
                ? PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
                : PendingIntentFlags.UpdateCurrent;

        private NotificationManager _notificationManager;
        private AlarmManager _alarmManager => Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;

        private Context CurrentContext;

        public NotificationService()
        {
            Initialize(Application.Context);
        }

        public NotificationService(Context context)
        {
            Initialize(context);
        }

        protected void Initialize(Context context)
        {
            try
            {
                CurrentContext = context;

                _notificationManager = CurrentContext.GetSystemService(Context.NotificationService) as NotificationManager;

                if (_notificationManager == null)
                    throw new ApplicationException("Notification service not found");

                CreateDefaultNotificationChannel(context);
                CreateNotificationChannelWithoutSoundAndVibration(context);
                CreateNotificationChannelWithoutSound(context);
                CreateNotificationChannelWithoutVibration(context);

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }

        private static NotificationTappedEventHandler onNotificationTapped;
        public event NotificationTappedEventHandler OnNotificationTapped
        {
            add => onNotificationTapped += value;
            remove => onNotificationTapped -= value;
        }

        public static void NotificationTapped(Intent intent)
        {
            if (intent?.Extras != null)
            {
                string payload = intent.GetStringExtra(NotificationConstans.PAYLOAD);
                int notificationId = intent.GetIntExtra(NotificationConstans.NOTIFICATION_ID, NotificationConstans.NOTIFICATION_ID_NOT_FOUND);

                // do nothing
                if (notificationId == NotificationConstans.NOTIFICATION_ID_NOT_FOUND)
                    return;

                var args = new NotificationEventArgs()
                {
                    NotificationId = notificationId,
                    Payload = payload,
                };

                onNotificationTapped?.Invoke(args);
            }
        }

        private static NotificationReceivedEventHandler onNotificationReceived;
        public event NotificationReceivedEventHandler OnNotificationReceived
        {
            add => onNotificationReceived += value;
            remove => onNotificationReceived -= value;
        }

        public static void NotificationReceived(int notificationId, string payload)
        {
            var args = new NotificationEventArgs()
            {
                NotificationId = notificationId,
                Payload = payload,
            };

            onNotificationReceived?.Invoke(args);
        }

        private static FirebasePushNotificationTokenEventHandler onTokenRefresh;
        public event FirebasePushNotificationTokenEventHandler OnTokenRefresh
        {
            add => onTokenRefresh += value;
            remove => onTokenRefresh -= value;
        }

        public static void TokenRefresh(string newToken)
        {
            onTokenRefresh?.Invoke(NotificationCenter.Current, new FirebasePushNotificationTokenEventArgs(newToken));
        }

        public void RegisterForPushNotifications()
        {

        }

        public void UnRegisterForPushNotifications()
        {

        }

        public async Task<string> GetTokenAsync()
        {
            var token = await FirebaseMessaging.Instance.GetToken();
            return token?.ToString();
        }

        public void Show(int notificationId, string title, string description, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            if (androidOptions == null)
                androidOptions = new AndroidOptions();

            NotificationRequest notificationRequest = new NotificationRequest()
            {
                NotificationId = notificationId,
                Title = title,
                Description = description,
                Payload = payload,
                Android = androidOptions
            };

            ShowNotification(CurrentContext, notificationRequest);
        }

        public void ShowHourly(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
            => Repeat(notificationId, title, description, Day.None, time, NotificationRepeat.Hourly, payload, androidOptions);

        public void ShowDaily(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
            => Repeat(notificationId, title, description, Day.None, time, NotificationRepeat.Daily, payload, androidOptions);

        public void ShowWeekly(int notificationId, string title, string description, Day weekDay, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
            => Repeat(notificationId, title, description, weekDay, time, NotificationRepeat.Weekly, payload, androidOptions);

        private void Repeat(int notificationId, string title, string description, Day weekDay, Time time, NotificationRepeat repeatInterval, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            androidOptions ??= new AndroidOptions();

            NotificationRequest notificationRequest = new NotificationRequest()
            {
                NotificationId = notificationId,
                Title = title,
                Description = description,
                CalledAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                RepeatInterval = repeatInterval,
                Repeats = true,
                Time = time,
                WeekDay = weekDay,
                Payload = payload,
                Android = androidOptions
            };

            RepeatNotification(CurrentContext, notificationRequest, true);
        }

        public void Schedule(int notificationId, string title, string description, DateTime dateTime, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null)
        {
            //NotityTime  a.k.a dateTime / scheduletime
            androidOptions ??= new AndroidOptions();

            //To Epoch timestamp
            var notifyTimeSinceEpoch = dateTime.GetMilliSecondsSinceEpoch();

            NotificationRequest notificationRequest = new NotificationRequest()
            {
                NotificationId = notificationId,
                Title = title,
                Description = description,
                NotifyTime = dateTime,
                NotifyTimeSinceEpoch = notifyTimeSinceEpoch,
                Payload = payload,
                Android = androidOptions
            };

            ScheduleNotification(CurrentContext, notificationRequest, true);
        }

        public void Cancel(int notificationId)
            => CancelNotification(notificationId);

        public void CancelAll()
            => CancelAllNotifications();

        public List<NotificationRequest> GetPendingNotificationRequests()
            => LoadScheduledNotifications(CurrentContext);

        private void CancelNotification(int notificationId)
        {
#if MONOANDROID
            if (Build.VERSION.SdkInt < BuildVersionCodes.Kitkat)
                return;
#elif ANDROID
            if (!OperatingSystem.IsAndroidVersionAtLeast(21))
                return;
#endif
            Intent intent = new Intent(CurrentContext, typeof(ScheduledNotificationReceiver));
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(CurrentContext, notificationId, intent, pendingIntentFlags);
            _alarmManager.Cancel(pendingIntent);

            var notificationManager = NotificationManager.FromContext(Application.Context);
            notificationManager.Cancel(notificationId);
            //_notificationManager.Cancel(notificationId);
            RemoveNotificationFromCache(CurrentContext, notificationId);
        }

        private void CancelAllNotifications()
        {
            var notificationManager = NotificationManager.FromContext(Application.Context);
            notificationManager.CancelAll();

            List<NotificationRequest> scheduledNotifications = LoadScheduledNotifications(CurrentContext);
            if (scheduledNotifications == null || scheduledNotifications.Count == 0)
                return;

            Intent intent = new Intent(CurrentContext, typeof(ScheduledNotificationReceiver));
            foreach (NotificationRequest notification in scheduledNotifications)
            {
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(CurrentContext, notification.NotificationId, intent, pendingIntentFlags);
                _alarmManager.Cancel(pendingIntent);
            }

            SaveScheduledNotifications(CurrentContext, new List<NotificationRequest>());
        }

        public void ShowNotification(Context context, NotificationRequest notificationRequest)
        {
            Notification notification = CreateNotification(context, notificationRequest);
            var notificationManager = NotificationManager.FromContext(Application.Context);
            notificationManager.Notify(notificationRequest.NotificationId, notification);

            var args = new NotificationEventArgs()
            {
                NotificationId = notificationRequest.NotificationId,
                Payload = notificationRequest.Payload,
            };
            onNotificationReceived?.Invoke(args);
        }

        private void ScheduleNotification(Context context, NotificationRequest notificationRequest, bool updateScheduledNotificationsCache)
        {
            string notificationRequestJson = notificationRequest.ToJson();
            Intent notificationIntent = new Intent(Application.Context, typeof(ScheduledNotificationReceiver));
            notificationIntent.PutExtra(NotificationConstans.NOTIFICATION_REQUEST, notificationRequestJson);
            PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, notificationRequest.NotificationId, notificationIntent, pendingIntentFlags);

            if (notificationRequest.Android.AllowWhileIdle)
                _alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, notificationRequest.NotifyTimeSinceEpoch, pendingIntent);
            else
                _alarmManager.SetExact(AlarmType.RtcWakeup, notificationRequest.NotifyTimeSinceEpoch, pendingIntent);

            if (updateScheduledNotificationsCache)
                SaveScheduledNotification(context, notificationRequest);
        }

        private void RepeatNotification(Context context, NotificationRequest notificationRequest, bool updateScheduledNotificationsCache)
        {
            try
            {
                string notificationRequestJson = notificationRequest.ToJson();
                Intent notificationIntent = new Intent(Application.Context, typeof(ScheduledNotificationReceiver));
                notificationIntent.PutExtra(NotificationConstans.NOTIFICATION_REQUEST, notificationRequestJson);
                notificationIntent.PutExtra(NotificationConstans.REPEAT, true);
                PendingIntent pendingIntent = PendingIntent.GetBroadcast(Application.Context, notificationRequest.NotificationId, notificationIntent, pendingIntentFlags);

                long repeatInterval = 0;

                switch (notificationRequest.RepeatInterval)
                {
                    case NotificationRepeat.Hourly:
                        repeatInterval = 60000 * 60;
                        break;
                    case NotificationRepeat.Daily:
                        repeatInterval = 60000 * 60 * 24;
                        break;
                    case NotificationRepeat.Weekly:
                        repeatInterval = 60000 * 60 * 24 * 7;
                        break;
                    default:
                        break;
                }

                long startTimeMilliSeconds = notificationRequest.CalledAt;
                if (notificationRequest.Time != null)
                {
                    Calendar calendar = Calendar.GetInstance(Android.Icu.Util.TimeZone.Default);
                    calendar.TimeInMillis = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    calendar.Set(CalendarField.HourOfDay, notificationRequest.Time.Hour);
                    calendar.Set(CalendarField.Minute, notificationRequest.Time.Minute);
                    calendar.Set(CalendarField.Second, notificationRequest.Time.Second);

                    if (notificationRequest.WeekDay != Day.None)
                        calendar.Set(CalendarField.DayOfWeek, (int)notificationRequest.WeekDay);

                    startTimeMilliSeconds = calendar.TimeInMillis;
                }

                //pastikan waktu mulai di masa yg akan datang
                long currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                while (startTimeMilliSeconds < currentTime)
                    startTimeMilliSeconds += repeatInterval;

                _alarmManager.SetInexactRepeating(AlarmType.RtcWakeup, startTimeMilliSeconds, repeatInterval, pendingIntent);

                if (updateScheduledNotificationsCache)
                {
                    notificationRequest.CalledAt = startTimeMilliSeconds;
                    SaveScheduledNotification(context, notificationRequest);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in : RepeatNotification");
                Console.WriteLine(ex);
            }
        }

        public void RescheduleNotifications(Context context)
        {
            List<NotificationRequest> scheduledNotifications = LoadScheduledNotifications(context);
            foreach (var notification in scheduledNotifications)
            {
                //Console.WriteLine("Id : " + notification.NotificationId + " | Description : " + notification.Description + " | RepeatInterval : " + notification.RepeatInterval.ToString());
                if (notification.RepeatInterval == NotificationRepeat.None)
                    ScheduleNotification(context, notification, false);
                else
                    RepeatNotification(context, notification, false);
            }
        }

        // https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/lock-statement
        private readonly object locker = new object();

        private List<NotificationRequest> LoadScheduledNotifications(Context context)
        {
            lock(locker)
            {
                List<NotificationRequest> scheduledNotifications = new List<NotificationRequest>();
                using (var sharedPreferences = GetSharedPreferences(context))
                {
                    string json = sharedPreferences.GetString(NotificationConstans.SCHEDULED_NOTIFICATIONS, null);
                    if (json != null)
                        scheduledNotifications = json.JsonToListObject<NotificationRequest>();

                    return scheduledNotifications;
                }
            }
        }

        public void SaveScheduledNotification(Context context, NotificationRequest notificationRequest)
        {
            //System.Diagnostics.Debug.WriteLine("SaveScheduledNotification - Start " + DateTime.Now);
            List<NotificationRequest> scheduledNotifications = LoadScheduledNotifications(context);

            int index = scheduledNotifications.FindIndex(x => x.NotificationId == notificationRequest.NotificationId);
            if (index >= 0)
                scheduledNotifications[index] = notificationRequest;
            else
                scheduledNotifications.Add(notificationRequest);

            SaveScheduledNotifications(context, scheduledNotifications);

            //System.Diagnostics.Debug.WriteLine("SaveScheduledNotification - End " + DateTime.Now);
        }


        private void SaveScheduledNotifications(Context context, List<NotificationRequest> scheduledNotifications)
        {
            lock(locker)
            {
                string json = scheduledNotifications.ToJson();

                using (var sharedPreferences = GetSharedPreferences(context))
                {
                    using (var editor = sharedPreferences.Edit())
                    {
                        editor.PutString(NotificationConstans.SCHEDULED_NOTIFICATIONS, json);

                        // https://stackoverflow.com/questions/5960678/whats-the-difference-between-commit-and-apply-in-sharedpreferences
                        // Unlike Commit(), which writes its preferences out to persistent storage synchronously,
                        // Apply() commits its changes to the in-memory SharedPreferences immediately but starts an asynchronous commit to disk and you won't be notified of any failures.
                        // Use Apply().
                        // It writes the changes to the RAM immediately and waits and writes it to the internal storage(the actual preference file) after.Commit writes the changes synchronously and directly to the file.
                        editor.Apply();
                    }
                };
            }
        }

        private ISharedPreferences GetSharedPreferences(Context context)
            => context.GetSharedPreferences(NotificationConstans.SCHEDULED_NOTIFICATIONS, FileCreationMode.Private);

        public void RemoveNotificationFromCache(Context context, int notificationId)
        {
            List<NotificationRequest> scheduledNotifications = LoadScheduledNotifications(context);

            foreach (NotificationRequest notification in scheduledNotifications)
            {
                if (notification.NotificationId == notificationId)
                {
                    scheduledNotifications.Remove(notification);
                    break;
                }
            }

            SaveScheduledNotifications(context, scheduledNotifications);
        }

        public Notification CreateNotification(Context context, NotificationRequest notificationRequest)
        {
            //CreateNotificationChannel();

            var notificationIntent = Application.Context.PackageManager?.GetLaunchIntentForPackage(Application.Context.PackageName ?? string.Empty);
            if (notificationIntent is null)
                throw new Exception("NotificationIntent is null");

            notificationIntent.SetFlags(ActivityFlags.SingleTop);
            notificationIntent.PutExtra(NotificationConstans.PAYLOAD, notificationRequest.Payload);
            notificationIntent.PutExtra(NotificationConstans.NOTIFICATION_ID, notificationRequest.NotificationId);
            PendingIntent pendingIntent = PendingIntent.GetActivity(context, notificationRequest.NotificationId, notificationIntent, pendingIntentFlags);

            var builder = new NotificationCompat.Builder(context, notificationRequest.Android.ChannelId)
                .SetContentTitle(notificationRequest.Title)
                .SetContentText(notificationRequest.Description)
                .SetStyle(new NotificationCompat.BigTextStyle().BigText(notificationRequest.Description))
                //.SetNumber(notificationRequest.BadgeNumber)
                .SetAutoCancel(notificationRequest.Android.AutoCancel)
                .SetOngoing(notificationRequest.Android.OnGoing)
                .SetDefaults((int)NotificationDefaults.All)// | (int)NotificationDefaults.Vibrate)
                .SetContentIntent(pendingIntent);



            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                builder.SetPriority((int)notificationRequest.Android.Priority);

                if (notificationRequest.Android.PlaySound)
                {
                    var soundUri = GetSoundUri(notificationRequest.Android.Sound);
                    if (soundUri != null)
                        builder.SetSound(soundUri);
                }
            }

            if (notificationRequest.Android.EnableVibration)
            {
                if (notificationRequest.Android.VibrationPatern != null)
                    builder.SetVibrate(notificationRequest.Android.VibrationPatern);
            }

            if (notificationRequest.Android.Color.HasValue)
                builder.SetColor(notificationRequest.Android.Color.Value);

            builder.SetSmallIcon(GetIcon(notificationRequest.Android.IconName));

            //https://stackoverflow.com/questions/33913952/android-notification-is-not-showing-colour-icon-in-marshmallow
            if (Build.VERSION.SdkInt == BuildVersionCodes.M)
            {
                builder.SetLargeIcon(GetIconBitmap(notificationRequest.Android.IconName, context));
                builder.SetSmallIcon(Android.Resource.Color.Transparent);
                builder.SetContentTitle($"HR.To : {notificationRequest.Title}");
            }

            if (notificationRequest.Android.TimeoutAfter.HasValue)
                builder.SetTimeoutAfter((long)notificationRequest.Android.TimeoutAfter.Value.TotalMilliseconds);

            //TODO ProgressBar

            var notification = builder.Build();
            return notification;
        }

        /// <summary>
        /// Create Notification Channel when API >= 26.
        /// Ref : https://stackoverflow.com/questions/29949501/android-show-notification-with-a-popup-on-top-of-any-application
        /// But remember that once you create the channel, you cannot change these settings and the user has final control of whether these behaviors are active. Other option is to uninstall and install application again
        /// </summary>
        /// <param name="context"></param>
        /// <param name="notificationChannelRequest"></param>
        private void CreateNotificationChannel(Context context, NotificationChannelRequest notificationChannelRequest)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            if (notificationChannelRequest == null)
                notificationChannelRequest = new NotificationChannelRequest();

            if (string.IsNullOrEmpty(notificationChannelRequest.Name))
                notificationChannelRequest.Name = "General";

            if (string.IsNullOrEmpty(notificationChannelRequest.Id))
                notificationChannelRequest.Id = NotificationConstans.DEFAULT_CHANNEL_ID;

            NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);

            NotificationChannel notificationChannel = notificationManager.GetNotificationChannel(notificationChannelRequest.Id);

            //update / create hanya saat diperlukan
            //if ((notificationChannel == null && request.ChannelAction == NotificationChannelAction.CreateIfNotExists) ||
            //    (notificationChannel != null && request.ChannelAction == NotificationChannelAction.Update))
            {
                notificationChannel = new NotificationChannel(notificationChannelRequest.Id, notificationChannelRequest.Name, notificationChannelRequest.Importance);
                notificationChannel.Description = notificationChannelRequest.Description;

                if (notificationChannelRequest.PlaySound)
                {
                    var soundUri = GetSoundUri(notificationChannelRequest.Sound);
                    if (soundUri != null)
                    {
                        var audioAttributes = new AudioAttributes.Builder().SetUsage(AudioUsageKind.Notification).SetContentType(AudioContentType.Music).Build();
                        notificationChannel.SetSound(soundUri, audioAttributes);
                    }
                }
                else
                {
                    notificationChannel.SetSound(null, null);
                }

                notificationChannel.EnableVibration(notificationChannelRequest.EnableVibration);
                if (notificationChannelRequest.VibrationPatern != null && notificationChannelRequest.VibrationPatern.Length > 0)
                    notificationChannel.SetVibrationPattern(notificationChannelRequest.VibrationPatern);

                //notificationChannel.LockscreenVisibility = NotificationVisibility.Public

                notificationChannel.EnableLights(notificationChannelRequest.EnableLights);
                if (notificationChannelRequest.EnableLights && notificationChannelRequest.ColorLight != null)
                    notificationChannel.LightColor = notificationChannelRequest.ColorLight;

                notificationChannel.SetShowBadge(notificationChannelRequest.ShowBadge);

                notificationManager.CreateNotificationChannel(notificationChannel);
            }
        }

        /// <summary>
        /// TODO cris
        /// </summary>
        private void CreateNotificationChannelGroup()
        {

        }

        private Android.Net.Uri GetSoundUri(string soundFileName)
        {
            if (string.IsNullOrEmpty(soundFileName))
                return null;

            if (soundFileName.Contains("://", StringComparison.InvariantCulture) == false)
                soundFileName = $"{ContentResolver.SchemeAndroidResource}://{Application.Context.PackageName}/raw/{soundFileName}";

            return Android.Net.Uri.Parse(soundFileName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="iconName"></param>
        /// <returns></returns>
        private int GetIcon(string iconName)
        {
            var iconId = 0;
            if (!string.IsNullOrEmpty(iconName))
                iconId = Application.Context.Resources.GetIdentifier(iconName, "drawable", Application.Context.PackageName);

            if (iconId != 0)
                return iconId;

            iconId = Application.Context.ApplicationInfo.Icon;
            if (iconId == 0)
                iconId = Application.Context.Resources.GetIdentifier("icon", "drawable", Application.Context.PackageName);

            return iconId;
        }

        private Bitmap GetIconBitmap(string iconName, Context context)
        {
            var iconId = GetIcon(iconName);
            var bitmap = BitmapFactory.DecodeResource(context.Resources, iconId);
            return bitmap;
        }

        private void CreateDefaultNotificationChannel(Context context)
        {
            CreateNotificationChannel(context, new NotificationChannelRequest()
            {
                Id = NotificationConstans.DEFAULT_CHANNEL_ID,
                Importance = NotificationImportance.High,
            });
        }

        private void CreateNotificationChannelWithoutSoundAndVibration(Context context)
        {
            CreateNotificationChannel(context, new NotificationChannelRequest()
            {
                Id = NotificationConstans.CHANNEL_ID_WITHOUT_SOUND_AND_VIBRATION,
                Name = "NoSoundNoVibration",
                Importance = NotificationImportance.Min,
                EnableVibration = false,
                PlaySound = false,
            });
        }

        private void CreateNotificationChannelWithoutSound(Context context)
        {
            CreateNotificationChannel(context, new NotificationChannelRequest()
            {
                Id = NotificationConstans.CHANNEL_ID_WITHOUT_SOUND,
                Name = "NoSound",
                Importance = NotificationImportance.Min,
                PlaySound = false,
            });
        }

        private void CreateNotificationChannelWithoutVibration(Context context)
        {
            CreateNotificationChannel(context, new NotificationChannelRequest()
            {
                Id = NotificationConstans.CHANNEL_ID_WITHOUT_VIBRATION,
                Name = "NoVibration",
                Importance = NotificationImportance.Min,
                EnableVibration = false,
            });
        }

        public Task<bool> IsNotificationsEnabled()
        {
            return _notificationManager is null
                ? Task.FromResult(false)
                : !((int)Build.VERSION.SdkInt >= 24)
                ? Task.FromResult(true)
                : Task.FromResult(_notificationManager.AreNotificationsEnabled());
        }


        public async Task<bool> RequestNotificationPermission(NotificationPermission permission = null)
        {
            permission ??= new NotificationPermission();

            if ((int)Build.VERSION.SdkInt < 33)
            {
                return false;
            }

            if (!permission.AskPermission)
            {
                return false;
            }

            var status = await Permissions.RequestAsync<NotificationPermissionAndroid>();
            return status == PermissionStatus.Granted;
        }


    }
}

using LocalNotifications;
using System.Diagnostics;

namespace LocalNotificationSample;

public partial class MainPage : ContentPage
{
    private int notificationId = 1;
    private int NotificationId => notificationId++;

    public MainPage()
    {
        InitializeComponent();
        InitializeLocalNotifications();
    }

    private void InitializeLocalNotifications()
    {
        // Subscribe to notification events
        LocalNotificationCenter.Current.OnNotificationReceived += OnNotificationReceived;
        LocalNotificationCenter.Current.OnNotificationTapped += OnNotificationTapped;
        LocalNotificationCenter.Current.OnTokenRefresh += OnTokenRefresh;
    }

    private void OnNotificationReceived(NotificationEventArgs e)
    {
        Debug.WriteLine($"[OnNotificationReceived] ID: {e.NotificationId}, Payload: {e.Payload}");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LogLabel.Text = $"Received notification ID: {e.NotificationId}\n{LogLabel.Text}";
        });
    }

    private void OnNotificationTapped(NotificationEventArgs e)
    {
        Debug.WriteLine($"[OnNotificationTapped] ID: {e.NotificationId}, Payload: {e.Payload}");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LogLabel.Text = $"Tapped notification ID: {e.NotificationId} (Payload: {e.Payload})\n{LogLabel.Text}";
        });
    }

    private void OnTokenRefresh(object source, FirebasePushNotificationTokenEventArgs e)
    {
        Debug.WriteLine($"[OnTokenRefresh] Token: {e.Token}");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LogLabel.Text = $"Firebase Token: {e.Token}\n{LogLabel.Text}";
        });
    }    // Show Notification Immediately
    private void OnShowNowClicked(object sender, EventArgs e)
    {
        try
        {
            LocalNotificationCenter.Current.Show(
                notificationId: NotificationId,
                title: "Show Now",
                description: "This notification shows immediately!",
                payload: "show_now_payload",
                androidOptions: new AndroidOptions
                {
                    IconName = "notification_icon",
                    ChannelId = "default_channel"
                },
                iOSOptions: new iOSOptions
                {
                    Sound = "default"
                });

            LogLabel.Text = $"Show Now notification sent\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Schedule Notification
    private void OnScheduleClicked(object sender, EventArgs e)
    {
        try
        {
            var scheduleTime = DateTime.Now.AddSeconds(30);
            LocalNotificationCenter.Current.Schedule(
                notificationId: NotificationId,
                title: "Scheduled Notification",
                description: $"Scheduled for {scheduleTime:HH:mm:ss}",
                dateTime: scheduleTime,
                payload: "scheduled_payload",
                androidOptions: new AndroidOptions
                {
                    IconName = "notification_icon",
                    ChannelId = "default_channel"
                },
                iOSOptions: new iOSOptions
                {
                    Sound = "default"
                });

            LogLabel.Text = $"Scheduled for {scheduleTime:HH:mm:ss}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Show Hourly Notification
    private void OnShowHourlyClicked(object sender, EventArgs e)
    {
        try
        {
            var time = new Time(DateTime.Now.Hour, DateTime.Now.Minute + 1); // Next minute
            LocalNotificationCenter.Current.ShowHourly(
                notificationId: NotificationId,
                title: "Hourly Notification",
                description: $"Repeats every hour at {time.Hour:D2}:{time.Minute:D2}",
                time: time,
                payload: "hourly_payload",
                androidOptions: new AndroidOptions
                {
                    IconName = "notification_icon",
                    ChannelId = "default_channel"
                },
                iOSOptions: new iOSOptions
                {
                    Sound = "default"
                });

            LogLabel.Text = $"Hourly notification set for :{time.Minute:D2}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Show Daily Notification
    private void OnShowDailyClicked(object sender, EventArgs e)
    {
        try
        {
            var time = new Time(DateTime.Now.Hour, DateTime.Now.Minute + 1); // Next minute
            LocalNotificationCenter.Current.ShowDaily(
                notificationId: NotificationId,
                title: "Daily Notification",
                description: $"Repeats every day at {time.Hour:D2}:{time.Minute:D2}",
                time: time,
                payload: "daily_payload",
                androidOptions: new AndroidOptions
                {
                    IconName = "notification_icon",
                    ChannelId = "default_channel"
                },
                iOSOptions: new iOSOptions
                {
                    Sound = "default"
                });

            LogLabel.Text = $"Daily notification set for {time.Hour:D2}:{time.Minute:D2}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Show Weekly Notification
    private void OnShowWeeklyClicked(object sender, EventArgs e)
    {
        try
        {
            var today = DateTime.Now.DayOfWeek;
            var day = today switch
            {
                DayOfWeek.Sunday => Day.Sunday,
                DayOfWeek.Monday => Day.Monday,
                DayOfWeek.Tuesday => Day.Tuesday,
                DayOfWeek.Wednesday => Day.Wednesday,
                DayOfWeek.Thursday => Day.Thursday,
                DayOfWeek.Friday => Day.Friday,
                DayOfWeek.Saturday => Day.Saturday,
                _ => Day.Monday
            };

            var time = new Time(DateTime.Now.Hour, DateTime.Now.Minute + 1);
            LocalNotificationCenter.Current.ShowWeekly(
                notificationId: NotificationId,
                title: "Weekly Notification",
                description: $"Repeats every {day} at {time.Hour:D2}:{time.Minute:D2}",
                weekDay: day,
                time: time,
                payload: "weekly_payload",
                androidOptions: new AndroidOptions
                {
                    IconName = "notification_icon",
                    ChannelId = "default_channel"
                },
                iOSOptions: new iOSOptions
                {
                    Sound = "default"
                });

            LogLabel.Text = $"Weekly notification set for {day} at {time.Hour:D2}:{time.Minute:D2}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Get Pending Notifications
    private void OnGetPendingClicked(object sender, EventArgs e)
    {
        try
        {
            var pending = LocalNotificationCenter.Current.GetPendingNotificationRequests();
            if (pending.Count > 0)
            {
                LogLabel.Text = $"Pending Notifications: {pending.Count}\n";
                foreach (var notification in pending)
                {
                    LogLabel.Text += $"- ID: {notification.NotificationId}, Title: {notification.Title}\n";
                }
            }
            else
            {
                LogLabel.Text = "No pending notifications\n" + LogLabel.Text;
            }
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Cancel Specific Notification
    private async void OnCancelSpecificClicked(object sender, EventArgs e)
    {
        try
        {
            string result = await DisplayPromptAsync("Cancel Notification", "Enter notification ID:", "Cancel", "Dismiss", keyboard: Keyboard.Numeric);
            if (!string.IsNullOrEmpty(result) && int.TryParse(result, out int id))
            {
                LocalNotificationCenter.Current.Cancel(id);
                LogLabel.Text = $"Cancelled notification ID: {id}\n{LogLabel.Text}";
            }
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Cancel All Notifications
    private void OnCancelAllClicked(object sender, EventArgs e)
    {
        try
        {
            LocalNotificationCenter.Current.CancelAll();
            LogLabel.Text = "All notifications cancelled\n" + LogLabel.Text;
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Request Notification Permission
    private async void OnRequestPermissionClicked(object sender, EventArgs e)
    {
        try
        {
            var permission = new NotificationPermission
            {
                AskPermission = true
            }; bool granted = await LocalNotificationCenter.Current.RequestNotificationPermission(permission);
            LogLabel.Text = $"Permission granted: {granted}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Check if Notifications Enabled
    private async void OnCheckEnabledClicked(object sender, EventArgs e)
    {
        try
        {
            bool enabled = await LocalNotificationCenter.Current.IsNotificationsEnabled();
            LogLabel.Text = $"Notifications enabled: {enabled}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Get Firebase Token
    private async void OnGetTokenClicked(object sender, EventArgs e)
    {
        try
        {
            string token = await LocalNotificationCenter.Current.GetTokenAsync();
            LogLabel.Text = $"Token: {token}\n{LogLabel.Text}";
        }
        catch (Exception ex)
        {
            LogLabel.Text = $"Error: {ex.Message}\n{LogLabel.Text}";
        }
    }

    // Clear Log
    private void OnClearLogClicked(object sender, EventArgs e)
    {
        LogLabel.Text = "Log cleared\n";
    }
}

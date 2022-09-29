using Android.App;

namespace LocalNotifications.Platform.Droid
{
    public class NotificationChannelRequest
    {
        public string Id { get; set; } = NotificationConstans.DEFAULT_CHANNEL_ID;
        public string Name { get; set; } = "General";
        public string Description { get; set; }
        public NotificationImportance Importance { get; set; } = NotificationImportance.Default;
        public NotificationChannelAction ChannelAction { get; set; } = NotificationChannelAction.None;
        public bool PlaySound { get; set; } = true;
        public string Sound { get; set; }
        public bool EnableVibration { get; set; } = true;
        public long[] VibrationPatern { get; set; }
        public bool EnableLights { get; set; } = true;
        public Android.Graphics.Color ColorLight { get; set; }
        public bool ShowBadge { get; set; } = true;
    }
}


using System;
namespace LocalNotifications
{
    public class AndroidOptions
    {
        public string ChannelId { get; set; } = NotificationConstans.DEFAULT_CHANNEL_ID;
        public string ChannelName { get; set; } = "General";
        public bool AllowWhileIdle { get; set; }
        public bool AutoCancel { get; set; } = true;
        public NotificationChannelAction ChannelAction { get; set; } = NotificationChannelAction.None;
        public bool PlaySound { get; set; }
        public string Sound { get; set; }
        public bool EnableVibration { get; set; } = true;
        public long[] VibrationPatern { get; set; }
        public string IconName { get; set; }
        public int? Color { get; set; }
        public bool EnableLights { get; set; } = true;
        public bool ShowBadge { get; set; } = true;
        public bool OnGoing { get; set; } = false;
        public NotificationPriority Priority { get; set; } = NotificationPriority.High;
        public TimeSpan? TimeoutAfter { get; set; }
    }
}

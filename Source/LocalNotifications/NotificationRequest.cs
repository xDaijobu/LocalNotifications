using System;

namespace LocalNotifications
{
    public class NotificationRequest
    {
        /// <summary>
        /// Harus Unique !
        /// ~if identifier is not unique, a new notification request object is not created~
        /// </summary>
        public int NotificationId { get; set; }

        /// <summary>
        /// Title for notification
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Detaisl for the notification
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the time the notification function was called.
        /// </summary>FBADGE
        public long CalledAt { get; set; }

        /// <summary>
        /// In DateTime
        /// </summary>
        public DateTime? LastCalledAt { get; set; }

        public long NotifyTimeSinceEpoch { get; set; }

        public DateTime? NotifyTime { get; set; }

        public bool Repeats { get; set; } = false;

        public NotificationRepeat RepeatInterval { get; set; }

        public Time Time { get; set; }

        public Day WeekDay { get; set; }

        /// <summary>
        /// Returning Payload when tapped on notification
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Android only..
        /// </summary>
        public AndroidOptions Android { get; set; } = new AndroidOptions();

        public iOSOptions iOS { get; set; } = new iOSOptions();
    }
}

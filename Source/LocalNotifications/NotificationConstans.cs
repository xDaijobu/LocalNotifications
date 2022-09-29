namespace LocalNotifications
{
    public static class NotificationConstans
    {
        private static string defaulChannelName = "LocalNotifications";
        /// <summary>
        /// Sound On &amp; Vibration On ( default )
        /// </summary>
        public static string DEFAULT_CHANNEL_ID = defaulChannelName + ".General";
        /// <summary>
        /// No sound &amp; No Vibration
        /// </summary>
        public static string CHANNEL_ID_WITHOUT_SOUND_AND_VIBRATION = defaulChannelName + ".NoSoundNoVibrationV2";
        /// <summary>
        /// No Sound
        /// </summary>
        public static string CHANNEL_ID_WITHOUT_SOUND = defaulChannelName + ".NoSoundV2";
        /// <summary>
        /// No Vibration
        /// </summary>
        public static string CHANNEL_ID_WITHOUT_VIBRATION = defaulChannelName + ".NoVibrationV2";

        public static string NOTIFICATION = "notification";
        public static string NOTIFICATION_ID = "notification_id";
        public static string SCHEDULED_NOTIFICATIONS = "scheduled_notifications";
        public static string NOTIFICATION_REQUEST = "notification_request";
        public static string REPEAT = "repeat";
        public static string PAYLOAD = "payload";
        public static string LOCAL_NOTIFICATIONS = "local_notifications";

        public static int NOTIFICATION_ID_NOT_FOUND = -1;
    }
}

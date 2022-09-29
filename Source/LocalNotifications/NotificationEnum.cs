namespace LocalNotifications
{
    public enum NotificationChannelAction
    {
        None,
        CreateIfNotExists,
        Update,
    }

    public enum NotificationRepeat
    {
        None,
        Hourly,
        Daily,
        Weekly,
    }

    public enum Day
    {
        None,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
    }

    /// <summary>
    /// Priority for notifications on Android 7.1 and lower
    /// </summary>
    public enum NotificationPriority
    {
        /// <summary>
        /// Lowest notification priority, these items might not be shown to the user except under special circumstances, such as detailed notification logs.
        /// </summary>
        Min = -2,

        /// <summary>
        /// Lower notification priority, for items that are less important.
        /// The UI may choose to show these items smaller, or at a different position in the list, compared with your app's Default items.
        /// </summary>
        Low = -1,

        /// <summary>
        /// If your application does not prioritize its own notifications, use this value for all notifications.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Higher notification priority, for more important notifications or alerts.
        /// The UI may choose to show these items larger, or at a different position in notification lists, compared with your app's Default items.
        /// </summary>
        High = 1,

        /// <summary>
        /// Highest notification priority, for your application's most important items that require the user's prompt attention or input.
        /// </summary>
        Max = 2,
    }
}

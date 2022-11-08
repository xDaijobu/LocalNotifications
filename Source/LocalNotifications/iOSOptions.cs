using System;
namespace LocalNotifications
{
    public class iOSOptions
    {
        //alert
        public bool PresentAlert { get; set; } = true;
        public bool PlaySound { get; set; } = true;
        public string Sound { get; set; } = string.Empty;
        public bool ShowBadge { get; set; } = true;
        /// <summary>
        /// iOS doesn't sum the badge numbers you send to the app
        /// </summary>
        public int? BadgeNumber { get; set; } = null;
        public bool HideForegroundalert { get; set; }
    }
}

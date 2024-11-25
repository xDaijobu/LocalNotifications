using Android;
using Android.OS;
using System;
using System.Collections.Generic;
#if MONOANDROID
using Xamarin.Essentials;
#else
using Microsoft.Maui.ApplicationModel;
#endif
namespace LocalNotifications.Platforms
{
    public partial class NotificationPermissionAndroid : Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions
        {
            get
            {
                var result = new List<(string androidPermission, bool isRuntime)>();
                if ((int)Build.VERSION.SdkInt >= 33)
                {
                    result.Add((Manifest.Permission.PostNotifications, true));
                }
                return result.ToArray();
            }
        }
    }
}

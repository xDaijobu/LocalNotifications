using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]

#if __ANDROID_33__
[assembly: UsesPermission(Manifest.Permission.PostNotifications)]
#endif

#if __ANDROID_31__
[assembly: UsesPermission(Manifest.Permission.ScheduleExactAlarm)]
[assembly: UsesPermission(Manifest.Permission.UseExactAlarm)]
#endif

[assembly: global::System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
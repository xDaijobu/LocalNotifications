using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]

#if __ANDROID_33__
[assembly: UsesPermission(Manifest.Permission.PostNotifications)]
#endif

[assembly: global::System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
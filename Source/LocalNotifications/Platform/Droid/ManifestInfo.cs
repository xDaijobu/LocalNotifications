using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]

#if ANDROID31_0_OR_GREATER
[assembly: UsesPermission(Manifest.Permission.ScheduleExactAlarm)]
#endif

#if NET6_0_OR_GREATER
[assembly: global::System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
#else
[assembly: LinkerSafe]
#endif
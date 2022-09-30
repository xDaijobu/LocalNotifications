using Android;
using Android.App;

[assembly: UsesPermission(Manifest.Permission.ReceiveBootCompleted)]
#if NET6_0_OR_GREATER
[assembly: global::System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
#else
[assembly: LinkerSafe]
#endif
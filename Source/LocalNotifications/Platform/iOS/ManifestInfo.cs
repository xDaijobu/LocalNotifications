using Foundation;

#if NET6_0_OR_GREATER
[assembly: global::System.Reflection.AssemblyMetadata("IsTrimmable", "True")]
#else
[assembly: LinkerSafe]
#endif
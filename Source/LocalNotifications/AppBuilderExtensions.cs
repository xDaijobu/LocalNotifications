#if NET8_0_OR_GREATER
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

namespace LocalNotifications;

public static class AppBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="isFirebase"></param>
    /// <param name="autoRegistration">Auto registers for push notifications if second parameter (a.k.a isFirebase) is true</param>
    /// <returns></returns>
    public static MauiAppBuilder UseLocalNotifications(this MauiAppBuilder builder, bool isFirebase, bool autoRegistration)
    {
        builder// https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle
              .ConfigureLifecycleEvents(events =>
              {
        #if ANDROID
	        events.AddAndroid(android => android
		        .OnCreate((activity, bundle) => OnNotificationTapped(activity.Intent))
		        .OnNewIntent((activity, intent) => OnNotificationTapped(intent)));

                static void OnNotificationTapped(Android.Content.Intent intent)
	            {
                    Platform.Droid.NotificationService.NotificationTapped(intent);
                }
        #elif IOS
	            events.AddiOS(iOS => iOS.FinishedLaunching((app, options) => InitLocalNotifications(options, isFirebase, autoRegistration)));

	            static bool InitLocalNotifications(Foundation.NSDictionary options, bool isFirebase, bool autoRegistration)
	            {
                    Platform.iOS.NotificationService.Initialize(options: options,
                                                                isFirebase: isFirebase,
                                                                autoRegistration: autoRegistration);
		            return true;
                }
        #endif
              });

        return builder;
    }
}
#endif

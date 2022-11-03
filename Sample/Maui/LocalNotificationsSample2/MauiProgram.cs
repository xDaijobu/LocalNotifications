using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;

namespace LocalNotificationsSample2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
            // https://learn.microsoft.com/en-us/dotnet/maui/fundamentals/app-lifecycle
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
				events.AddAndroid(android => android
						.OnCreate((activity, bundle) => OnNotificationTapped(activity.Intent))
						.OnNewIntent((activity, intent) => OnNotificationTapped(intent)));

                static void OnNotificationTapped(Android.Content.Intent intent)
				{
                    LocalNotifications.Platform.Droid.NotificationService.NotificationTapped(intent);
                }
#elif IOS
				events.AddiOS(iOS => iOS.FinishedLaunching((app, options) => InitLocalNotifications(options)));

				static bool InitLocalNotifications(Foundation.NSDictionary options)
				{
                    LocalNotifications.Platform.iOS.NotificationService.Initialize(options: options,
																				   isFirebase: false,
																				   autoRegistration: true);
					return true;
                }
#endif
			}); ;

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}


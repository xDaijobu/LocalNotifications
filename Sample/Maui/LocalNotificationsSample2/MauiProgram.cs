using LocalNotifications;
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
			.UseLocalNotifications(isFirebase: false, autoRegistration: false);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}


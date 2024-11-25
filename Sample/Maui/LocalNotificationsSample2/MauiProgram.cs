using LocalNotifications;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
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
				fonts.AddFont("OpenSansRegular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSansSemibold.ttf", "OpenSansSemibold");
			})
			.UseLocalNotifications(isFirebase: false, autoRegistration: false);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}


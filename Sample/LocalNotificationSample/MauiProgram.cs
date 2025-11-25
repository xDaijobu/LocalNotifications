using LocalNotifications;

namespace LocalNotificationSample;

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
            .UseLocalNotifications(isFirebase: false, autoRegistration: true);

#if DEBUG && !IOS
		builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}

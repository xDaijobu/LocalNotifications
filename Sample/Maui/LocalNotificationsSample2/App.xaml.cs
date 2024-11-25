using LocalNotifications;

namespace LocalNotificationsSample2;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();

        LocalNotificationCenter.Current.OnNotificationTapped += OnNotificationTapped;
        LocalNotificationCenter.Current.OnNotificationReceived += OnNotificationReceived;
        LocalNotificationCenter.Current.OnTokenRefresh += (s, e) =>
        {
            System.Diagnostics.Debug.WriteLine("Current Token : " + e.Token);
        };
    }

    protected async void OnNotificationTapped(NotificationEventArgs e)
    {
        await Shell.Current?.CurrentPage?.DisplayAlert("OnNotificationTapped",
                                                        GetText(e),
                                                        "Ok");
    }

    protected async void OnNotificationReceived(NotificationEventArgs e)
    {
        await Shell.Current?.CurrentPage?.DisplayAlert("OnNotificationReceived",
                                                       GetText(e),
                                                       "Ok");
    }

    protected string GetText(NotificationEventArgs e)
    {
        return $"NotificationId: {e.NotificationId}\n" +
               $"Payload: {(string.IsNullOrEmpty(e.Payload) ? "Null/Empty" : e.Payload)}";
    }
}


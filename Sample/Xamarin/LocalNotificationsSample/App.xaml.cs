using LocalNotifications;
using Xamarin.Forms;

namespace LocalNotificationsSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NotificationPage();
            NotificationCenter.Current.OnNotificationTapped += OnNotificationTapped;
            NotificationCenter.Current.OnNotificationReceived += OnNotificationReceived;
            NotificationCenter.Current.OnTokenRefresh += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine("Current Token : " + e.Token);
            };
        }

        protected async void OnNotificationTapped(NotificationEventArgs e)
        {
            await (Current as App).MainPage.DisplayAlert("OnNotificationTapped",
                                                         GetText(e),
                                                         "Ok");
        }

        protected async void OnNotificationReceived(NotificationEventArgs e)
        {
            await (Current as App).MainPage.DisplayAlert("OnNotificationReceived",
                                                         GetText(e),
                                                         "Ok");
        }

        protected string GetText(NotificationEventArgs e)
        {
            return $"NotificationId: {e.NotificationId}\n" +
                   $"Payload: {(string.IsNullOrEmpty(e.Payload) ? "Null/Empty" : e.Payload)}";
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}


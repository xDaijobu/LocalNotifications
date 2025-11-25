using LocalNotifications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LocalNotificationsSample4;

public partial class NotificationPage : ContentPage
{
    public NotificationPage()
    {
        InitializeComponent();
        BindingContext = new NotificationViewModel();
    }
}

public class NotificationViewModel : BaseViewModel
{
    private Page CurrentPage => (Application.Current as App).MainPage;

    ObservableCollection<NotificationRequest> pendingNotifications;
    public ObservableCollection<NotificationRequest> PendingNotifications
    {
        get => pendingNotifications;
        set => SetValue(ref pendingNotifications, value);
    }

    NotificationRequest selectedNotification;
    public NotificationRequest SelectedNotification
    {
        get => selectedNotification;
        set => SetValue(ref selectedNotification, value);
    }

    int notificationId = 1;
    public int NotificationId => notificationId++;

    public NotificationViewModel()
    {
        PendingNotifications = new ObservableCollection<NotificationRequest>();
        GetNotifications();


        LocalNotificationCenter.Current.OnNotificationReceived += (e) =>
        {
            Debug.WriteLine("NotificationReceived: NotificationId " + e.NotificationId);

            // re-get notifications
            GetNotifications();
        };
        
        LocalNotificationCenter.Current.OnNotificationTapped += (e) =>
        {
            Debug.WriteLine("NotificationTapped: NotificationId " + e.NotificationId);

        };
    }

    public Command ShowNotificationCommand => new Command(() =>
    {

        LocalNotificationCenter.Current.Show(notificationId: NotificationId,
                                        title: "ShowNow",
                                        description: "Hello World",
                                        payload: "",
                                        androidOptions: GetAndroidOptions());
    });

    public Command ScheduleNotificationCommand => new Command(async () =>
    {
        int value = 30;
        LocalNotificationCenter.Current.Schedule(notificationId: NotificationId,
                                            title: $"Schedule: {DateTime.Now.AddSeconds(value)}",
                                            description: "Hello World",
                                            dateTime: DateTime.Now.AddSeconds(value),
                                            payload: "",
                                            androidOptions: GetAndroidOptions());

        GetNotifications();
        await CurrentPage.DisplayAlert("Schedule Notification", $"{value} dtk dr skrng notifikasinya akan muncul", "Ok").ConfigureAwait(false);
    });

    public Command CancelNotificationCommand => new Command(async () =>
    {
        if (SelectedNotification == null)
        {
            await CurrentPage.DisplayAlert("Cancel Notification", "Notification not found", "Ok");
            return;
        }

        LocalNotificationCenter.Current.Cancel(SelectedNotification.NotificationId);
        SelectedNotification = null;
        GetNotifications();

        await CurrentPage.DisplayAlert("Cancel Notification", "Success", "Ok");
    });

    public Command CancelAllNotificationsCommand => new Command(async () =>
    {
        LocalNotificationCenter.Current.CancelAll();

        SelectedNotification = null;
        GetNotifications();
        await CurrentPage.DisplayAlert("Cancel All Notifications", "Success", "Ok");
    });

    private AndroidOptions GetAndroidOptions()
    {
        return new AndroidOptions()
        {
            AllowWhileIdle = true,
            Priority = NotificationPriority.Default,
            ChannelId = NotificationConstans.DEFAULT_CHANNEL_ID,
        };
    }

    public void GetNotifications()
    {
        PendingNotifications = new ObservableCollection<NotificationRequest>(LocalNotificationCenter.Current.GetPendingNotificationRequests());
    }
}

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    protected void SetValue<T>(ref T backingfield, T value, [CallerMemberName] string PropertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(backingfield, value)) return;
        backingfield = value;

        OnPropertyChanged(PropertyName);
    }
}
# Local Notifications

## Sample App
* [Xamarin](https://github.com/xDaijobu/LocalNotifications/tree/main/Sample/Xamarin) 
* [MAUI](https://github.com/xDaijobu/LocalNotifications/tree/main/Sample/Maui/LocalNotificationsSample2)

<img width="307" alt="Screenshot 2022-11-03 at 14 07 09" src="https://user-images.githubusercontent.com/22674537/199664085-a547575f-1506-4249-bfaf-5417df8dcbad.png"><img width="373" alt="Screenshot 2022-11-03 at 14 10 19" src="https://user-images.githubusercontent.com/22674537/199664456-dd9e8b62-c9c3-42c2-a91b-51e716861f57.png">

**Usage**
```csharp
int notificationId = 1;
public int NotificationId => notificationId++;

// Show Local Notification
NotificationCenter.Current.Show(notificationId: NotificationId,
                                title: "ShowNow",
                                description: "Hello World",
                                payload: "",
                                androidOptions: new AndroidOptions(),
				iOSOptions = new iOSOptions());

// Show Hourly / Daily / Weekly Local Notification
NotificationCenter.Current.ShowHourly(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
NotificationCenter.Current.ShowDaily(int notificationId, string title, string description, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);
NotificationCenter.Current.ShowWeekly(int notificationId, string title, string description, Day weekDay, Time time, string payload, AndroidOptions androidOptions = null, iOSOptions iOSOptions = null);

// Schedule Local Notification
int value = 30;
NotificationCenter.Current.Schedule(notificationId: NotificationId,
                                    title: $"Schedule: {DateTime.Now.AddSeconds(value)}",
                                    description: "Hello World",
                                    dateTime: DateTime.Now.AddSeconds(value),
                                    payload: "",
                                    androidOptions: GetAndroidOptions());

// Cancel Local Notification
NotificationCenter.Current.Cancel(notificationId: 9999);

// Cancel All Notification
NotificationCenter.Current.CancelAll();

// Get Pending Notification Requests
var pendingNotifications = await NotificationCenter.Current.GetPendingNotificationRequests();

// Events
NotificationCenter.Current.OnNotificationReceived += (e) =>
{
    Debug.WriteLine("OnNotificationReceived: NotificationId " + e.NotificationId);
};

NotificationCenter.Current.OnNotificationTapped += (e) =>
{
    Debug.WriteLine("OnNotificationTapped: NotificationId " + e.NotificationId);
};

// Firebase ~
NotificationCenter.Current.OnTokenRefresh += (e) => 
{
    Debug.WriteLine("Firebase Token: " + e.Token);
};
```
## Getting Started

**Platform Specific Notes [MAUI]**

To receive the Local Notification tap event. Include the following code in the CreateMauiApp() method of MauiProgram:
```csharp
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

		return builder.Build();
	}
```


**Platform Specific Notes [Xamarin]**

*Android*

The project should target Android framework 11.0+

*Setup*

To receive the Local Notification tap event. Incldue the following code in the OnNewIntent() method of MainActivity:

```csharp
public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState)
	{
	    	.....		
		LoadApplication(new App());
		.....	
		LocalNotifications.Platform.Droid.NotificationService.NotificationTapped(Intent);
	}

	protected override void OnNewIntent(Intent intent)
	{
		LocalNotifications.Platform.Droid.NotificationService.NotificationTapped(intent);
		base.OnNewIntent(intent);
	}
}
```

*iOS*

*Setup*

You must get permission from the user to allow the app to show local notifications. Also, to receive the Local Notification tap event. Include the following code in the FinishedLaunching() method of AppDelegate:

```csharp
public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
{        
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // The user will be asked when showing the first notification.
            LocalNotifications.Platform.iOS.NotificationService.Initialize(options: options,
                                                                           isFirebase: false,
                                                                           autoRegistration: true);

            LoadApplication(new App());
            return base.FinishedLaunching(app, options);
        }
}
```

## Project Structure

| Namespace | Description |
|--------------|--------------|
| LocalNotifications | ~ |
| LocalNotifications.Platform.Droid | ~ |
| LocalNotifications.Platform.iOS | ~ |

## Further information

For more information please visit:

- Github repository: https://github.com/xDaijobu/LocalNotifications



# LocalNotifications for .NET MAUI

[![NuGet](https://img.shields.io/nuget/v/LocalNotifications)](https://www.nuget.org/packages/LocalNotifications/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)

A cross-platform local notifications plugin for **.NET MAUI** — supports Android and iOS with scheduling, repeating, and Firebase push notification integration.

---

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Getting Started](#getting-started)
- [Usage](#usage)
- [Events](#events)
- [Firebase Push Notifications](#firebase-push-notifications)
- [Platform Notes](#platform-notes)
- [Sample App](#sample-app)
- [License](#license)

---

## Features

- 🔔 **Show** notifications immediately
- ⏰ **Schedule** notifications at a specific date/time
- 🔁 **Repeating** notifications — hourly, daily, or weekly
- 📋 **Manage** pending notifications (list, cancel one, cancel all)
- 🔑 **Permission** handling for Android 13+ (POST_NOTIFICATIONS)
- 🔥 **Firebase Cloud Messaging** integration (Android & iOS)

---

## Installation

Install via NuGet Package Manager:

```
dotnet add package LocalNotifications
```

Or search for **LocalNotifications** in the NuGet Package Manager in Visual Studio.

---

## Getting Started

### 1. Register in `MauiProgram.cs`

Add `.UseLocalNotifications()` in your `CreateMauiApp()` method:

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
        .UseLocalNotifications(isFirebase: false, autoRegistration: true);

    return builder.Build();
}
```

| Parameter | Description |
|---|---|
| `isFirebase` | Set to `true` to enable Firebase Cloud Messaging |
| `autoRegistration` | Set to `true` to automatically register lifecycle events |

### 2. (Alternative) Manual Lifecycle Registration

If you need more control, you can manually configure platform lifecycle events instead of using `autoRegistration`:

```csharp
builder.ConfigureLifecycleEvents(events =>
{
#if ANDROID
    events.AddAndroid(android => android
        .OnCreate((activity, bundle) => OnNotificationTapped(activity.Intent))
        .OnNewIntent((activity, intent) => OnNotificationTapped(intent)));

    static void OnNotificationTapped(Android.Content.Intent intent)
    {
        LocalNotifications.Platform.NotificationService.NotificationTapped(intent);
    }
#elif IOS
    events.AddiOS(iOS => iOS.FinishedLaunching((app, options) =>
    {
        LocalNotifications.Platform.NotificationService.Initialize(
            options: options,
            isFirebase: false,
            autoRegistration: true);
        return true;
    }));
#endif
});
```

---

## Usage

### Show a Notification Immediately

```csharp
LocalNotificationCenter.Current.Show(
    notificationId: 1,
    title: "Hello!",
    description: "This notification shows immediately.",
    payload: "my_payload",
    androidOptions: new AndroidOptions { IconName = "notification_icon" },
    iOSOptions: new iOSOptions { Sound = "default" });
```

### Schedule a Notification

```csharp
LocalNotificationCenter.Current.Schedule(
    notificationId: 2,
    title: "Reminder",
    description: "This fires in 30 seconds.",
    dateTime: DateTime.Now.AddSeconds(30),
    payload: "scheduled_payload",
    androidOptions: new AndroidOptions(),
    iOSOptions: new iOSOptions());
```

### Repeating Notifications

```csharp
var time = new Time(hour: 9, minute: 0);

// Every hour at :00
LocalNotificationCenter.Current.ShowHourly(
    notificationId: 3, title: "Hourly", description: "Every hour",
    time: time, payload: "hourly");

// Every day at 09:00
LocalNotificationCenter.Current.ShowDaily(
    notificationId: 4, title: "Daily", description: "Every day at 9 AM",
    time: time, payload: "daily");

// Every Monday at 09:00
LocalNotificationCenter.Current.ShowWeekly(
    notificationId: 5, title: "Weekly", description: "Every Monday at 9 AM",
    weekDay: Day.Monday, time: time, payload: "weekly");
```

### Cancel Notifications

```csharp
// Cancel a specific notification
LocalNotificationCenter.Current.Cancel(notificationId: 1);

// Cancel all notifications
LocalNotificationCenter.Current.CancelAll();
```

### Get Pending Notifications

```csharp
var pending = LocalNotificationCenter.Current.GetPendingNotificationRequests();
foreach (var n in pending)
{
    Debug.WriteLine($"Pending: ID={n.NotificationId}, Title={n.Title}");
}
```

### Request Permission

```csharp
bool granted = await LocalNotificationCenter.Current.RequestNotificationPermission(
    new NotificationPermission { AskPermission = true });
```

### Check if Notifications Are Enabled

```csharp
bool enabled = await LocalNotificationCenter.Current.IsNotificationsEnabled();
```

---

## Events

```csharp
// Fired when a notification is received while the app is in the foreground
LocalNotificationCenter.Current.OnNotificationReceived += (e) =>
{
    Debug.WriteLine($"Received: ID={e.NotificationId}");
};

// Fired when the user taps a notification
LocalNotificationCenter.Current.OnNotificationTapped += (e) =>
{
    Debug.WriteLine($"Tapped: ID={e.NotificationId}, Payload={e.Payload}");
};

// Fired when Firebase token is refreshed
LocalNotificationCenter.Current.OnTokenRefresh += (source, e) =>
{
    Debug.WriteLine($"Firebase Token: {e.Token}");
};
```

---

## Firebase Push Notifications

To enable Firebase Cloud Messaging, pass `isFirebase: true` when registering:

```csharp
.UseLocalNotifications(isFirebase: true, autoRegistration: true);
```

### Android Setup

1. Add `google-services.json` to your Android project
2. Set its **Build Action** to `GoogleServicesJson`
3. Add the following permission to `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
```

### iOS Setup

1. Add `GoogleService-Info.plist` to your iOS project (Build Action: `BundleResource`)
2. In `Info.plist`, enable **Background Modes** → **Remote Notifications**
3. Add `FirebaseAppDelegateProxyEnabled` = `NO` in `Info.plist`
4. In `Entitlements.plist`, enable **Push Notifications**
5. Configure the required Apple Push Notification certificates and provisioning profile

### Get Firebase Token

```csharp
string token = await LocalNotificationCenter.Current.GetTokenAsync();
```

---

## Platform Notes

### iOS

> ⚠️ iOS imposes a limit of **64 pending notifications**. Only the 64 soonest-firing notifications are kept.

### Android

> ⚠️ Some Android OEMs customize the OS in ways that may prevent background scheduling from working reliably. See [Don't Kill My App](https://dontkillmyapp.com/) for details.

---

## Sample App

Check out the [sample app](https://github.com/xDaijobu/LocalNotifications/tree/main/Sample/LocalNotificationSample) for a complete working example.

| Platform | Screenshots |
|:---|:---:|
| Android | <img width="300" alt="Android screenshot 1" src="https://user-images.githubusercontent.com/22674537/199664085-a547575f-1506-4249-bfaf-5417df8dcbad.png"> <img width="300" alt="Android screenshot 2" src="https://user-images.githubusercontent.com/22674537/199664456-dd9e8b62-c9c3-42c2-a91b-51e716861f57.png"> |
| iOS | <img width="300" alt="iOS screenshot 1" src="https://user-images.githubusercontent.com/22674537/200498405-03ebc105-2728-4bb4-bccf-573266f12ed7.png"> <img width="300" alt="iOS screenshot 2" src="https://user-images.githubusercontent.com/22674537/200498521-88d915ac-bc30-4e6b-b90a-7126248f73a8.png"> |

---

## License

This project is licensed under the [MIT License](LICENSE.md).

For more information, visit the [GitHub repository](https://github.com/xDaijobu/LocalNotifications).

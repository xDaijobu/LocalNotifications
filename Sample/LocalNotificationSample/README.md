# LocalNotificationSample

This is a sample MAUI application demonstrating all the APIs available in the LocalNotifications library.

## Features Demonstrated

### Show Notifications
- **Show Now**: Display an immediate notification
- **Schedule**: Schedule a notification for 30 seconds in the future

### Repeating Notifications
- **Hourly**: Show notification every hour at a specific minute
- **Daily**: Show notification every day at a specific time
- **Weekly**: Show notification every week on a specific day and time

### Manage Notifications
- **Get Pending**: List all pending notification requests
- **Cancel Specific**: Cancel a specific notification by ID
- **Cancel All**: Cancel all pending notifications

### Permissions & Settings
- **Request Permission**: Request notification permissions from the system
- **Check if Enabled**: Check if notifications are enabled for the app
- **Get Firebase Token**: Get the Firebase Cloud Messaging token (if Firebase is enabled)

### Events
The app subscribes to these events and logs them:
- `OnNotificationReceived`: Triggered when a notification is received
- `OnNotificationTapped`: Triggered when a notification is tapped
- `OnTokenRefresh`: Triggered when the Firebase token refreshes

## Project Reference

This project references the LocalNotifications library via project reference:
```xml
<ProjectReference Include="..\..\Source\LocalNotifications\LocalNotifications.csproj" />
```

## Platform Support

- ✅ Android (API 21+)
- ✅ iOS (12.2+)

## Running the App

1. Open the solution in Visual Studio or Visual Studio for Mac
2. Select either Android or iOS as the target platform
3. Run the app
4. Try different notification features
5. Watch the Event Log at the bottom to see when notifications are received or tapped

## Notes

- Notifications may not show when the app is in the foreground on some platforms
- On Android 13+, you need to grant notification permissions first
- Scheduled and repeating notifications will persist even after closing the app

using System;
#if XAMARINIOS
using Firebase.CloudMessaging;
#endif
using Foundation;

namespace LocalNotifications.Platform.iOS
{
#if XAMARINIOS
    public class FirebaseMessagingDelegate : NSObject, IMessagingDelegate
    {
        readonly Action<string> onToken;

        public FirebaseMessagingDelegate(Action<string> onToken)
        {
            this.onToken = onToken;
        }

        [Export("messaging:didReceiveRegistrationToken:")]
        public void DidReceiveRegistrationToken(Messaging messaging, string fcmToken)
            => this.onToken(fcmToken);
    }
#elif IOS
    public class FirebaseMessagingDelegate
    {
        readonly Action<string> onToken;

        public FirebaseMessagingDelegate(Action<string> onToken)
        {
            this.onToken = onToken;
        }
    }
#endif
}

using System;
using Firebase.CloudMessaging;
using Foundation;

namespace LocalNotifications.Platform.iOS
{
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
}

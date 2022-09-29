using System;
using System.Threading.Tasks;
using LocalNotifications.UnitTests.Mocks;
using NUnit.Framework;
using Xamarin.Forms;

namespace LocalNotifications.UnitTests
{
    public class Notification_Tests
    {
        readonly MockNotificationService notificationService = new MockNotificationService();
        readonly AndroidOptions androidOptions = new AndroidOptions();
        readonly iOSOptions iOSOptions = new iOSOptions();
        public Notification_Tests()
        {
            Assert.False(androidOptions.AllowWhileIdle);
            Assert.True(androidOptions.AutoCancel);
            Assert.False(androidOptions.PlaySound);
            Assert.True(androidOptions.EnableVibration);
            Assert.True(androidOptions.EnableLights);
            Assert.True(androidOptions.ShowBadge);
            Assert.False(androidOptions.OnGoing);
            Assert.AreEqual(NotificationPriority.High, androidOptions.Priority);

            Assert.IsTrue(iOSOptions.PresentAlert);
            Assert.IsTrue(iOSOptions.PlaySound);
            Assert.IsTrue(iOSOptions.ShowBadge);
        }

        [Test]
        public void DisplayNotification_PlatformNotSupportedException()
        {
            Assert.ThrowsAsync<PlatformNotSupportedException>(() => Task.Run(() => notificationService.Show(1, "test", "received?", string.Empty, null, null)));
        }

        [Test]
		public void TestOnNotificationReceived()
        {
            var fired = false;
            var args = new NotificationEventArgs();
            notificationService.OnNotificationReceived += (e) => fired = e == args;
            notificationService.RaiseNotificationReceived(args);

            Assert.IsTrue(fired);
        }

        [Test]
        public void TestOnNotificationTapped()
        {
            var fired = false;
            var args = new NotificationEventArgs();
            notificationService.OnNotificationTapped += (e) => fired = e == args;
            notificationService.RaiseNotificationTapped(args);

            Assert.IsTrue(fired);
        }

        [Test]
        public void TestOnTokenRefresh()
        {
            var fired = false;
            var args = new FirebasePushNotificationTokenEventArgs("1");
            notificationService.OnTokenRefresh += (s, e) => fired = e == args;
            notificationService.RaiseTokenRefresh(args);

            Assert.IsTrue(fired);
        }
    }
}

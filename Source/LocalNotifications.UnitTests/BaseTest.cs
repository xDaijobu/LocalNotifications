//using System;
//using System.Globalization;
//using System.Threading;
//using LocalNotifications.UnitTests.Mocks;
//using Xamarin.Forms;

//namespace LocalNotifications.UnitTests
//{
//    public abstract class BaseTest : IDisposable
//    {
//        readonly CultureInfo defaultCulture, defaultUICulture;
//        bool isDisposed;

//		protected BaseTest()
//		{
//			defaultCulture = Thread.CurrentThread.CurrentCulture;
//			defaultUICulture = Thread.CurrentThread.CurrentUICulture;

//			Device.PlatformServices = new MockPlatformServices();

//			//DispatcherProvider.SetCurrent(new MockDispatcherProvider());
//			//DeviceDisplay.SetCurrent(null);
//		}

//		~BaseTest() => Dispose(false);

//        public void Dispose()
//        {
//            Dispose(true);
//            GC.SuppressFinalize(this);
//        }

//		protected virtual void Dispose(bool isDisposing)
//		{
//			if (isDisposed)
//				return;

//			isDisposed = true;
//		}
//	}
//}

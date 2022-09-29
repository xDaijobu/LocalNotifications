//using System;
//using Xamarin.Forms;

//namespace LocalNotifications.UnitTests.Mocks
//{
//	sealed class MockDispatcherProvider : IDispatcherProvider
//	{
//		public IDispatcher GetDispatcher(object context)
//		{
//            return new MockDispatcher();
//        }

//		public class MockDispatcher : IDispatcher
//		{
//			public void BeginInvokeOnMainThread(Action action)
//			{
//				Device.BeginInvokeOnMainThread(action);
//			}

//			bool IDispatcher.IsInvokeRequired => Device.IsInvokeRequired;
//		}
//	}
//}

using System;
using Android.App;
using Android.Content;
using Android.Util;
using Firebase.Iid;
using Plugin.Settings;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	[Service]
	public class FCMRegistrationService : IntentService
	{
		private const string TAG = "FCMRegistrationService";
		static object locker = new object();

		protected override void OnHandleIntent(Intent intent)
		{
			try
			{
				lock (locker)
				{
					var instanceId = FirebaseInstanceId.Instance;
					var token = instanceId.Token;

					if (string.IsNullOrEmpty(token))
						return;

#if DEBUG
                    instanceId.DeleteToken(token, "");
                    instanceId.DeleteInstanceId();
#endif
					
					//var refreshedToken = FirebaseInstanceId.Instance.GetToken(token, "");
					token = FirebaseInstanceId.Instance.Token;

					AppServices appServices = new AppServices(this);
					var uniqueDeviceId = appServices.UniqueDeviceId;

					Shared.SendRegistrationToServer(token, "android", uniqueDeviceId);
				}
			}
			catch (Exception e)
			{
				Log.Debug(TAG, e.Message);
			}
		}
	}
}

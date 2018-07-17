using System;
using Android.App;
using Android.Util;
using Firebase.Iid;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	[Service]
	[IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
	public class MyFirebaseIIDService : FirebaseInstanceIdService
	{
		const string TAG = "MyFirebaseIIDService";
		public override void OnTokenRefresh()
		{
			try
			{
				var refreshedToken = FirebaseInstanceId.Instance.Token;
				Log.Debug(TAG, "Refreshed token: " + refreshedToken);

				AppServices appServices = new AppServices(this);
				var uniqueDeviceId = appServices.UniqueDeviceId;
 				Shared.SendRegistrationToServer(refreshedToken, "android", uniqueDeviceId);
			}
			catch (Exception e)
			{
				Log.Debug(TAG, e.Message);
			}
		}
	}
}

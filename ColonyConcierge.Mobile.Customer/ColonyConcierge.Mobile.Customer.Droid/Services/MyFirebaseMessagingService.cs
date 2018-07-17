using System;
using Android.App;
using Android.Content;
using Android.Util;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;
using Firebase.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	[Service]
	[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
	public class MyFirebaseMessagingService : FirebaseMessagingService
	{
		const string TAG = "MyFirebaseMsgService";
		public override void OnMessageReceived(RemoteMessage message)
		{
			var from = message.From;
			var data = message.Data;
			var body = message.GetNotification().Body;
			var title = message.GetNotification().Title;

			string messageString = string.Empty;
			LogisticsNotification logisticsNotification = null;

			// Instantiate the builder and set notification elements:
			var tInent = new Intent(this, typeof(MainActivity));

			try
			{
				foreach (var item in data)
				{
					var value = item.Value;
					tInent.PutExtra(item.Key, item.Value);
					if (item.Key.Equals("logistics"))
					{
						logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(value);
					}
					else if (item.Key.Equals("message"))
					{
						messageString = value;
					}
				}
			}
			catch (Exception) { }

			var pendingIntent = PendingIntent.GetActivity(this, 0, tInent, PendingIntentFlags.UpdateCurrent);

			if (logisticsNotification != null)
			{
				//Check app is open??? When user pause app same time received notification
				var isOpen = false;
				try
				{
					var appServices = DependencyService.Get<IAppServices>() as AppServices;
					isOpen = appServices.MainActivity != null && !appServices.MainActivity.IsFinishing && !appServices.MainActivity.IsPaused && !appServices.MainActivity.IsDestroyed;
				}
				catch (Exception)
				{
					isOpen = false;
				}
				if (isOpen)
				{
					var appServices = DependencyService.Get<IAppServices>() as AppServices;
					if (appServices != null)
					{
						Device.BeginInvokeOnMainThread(async () =>
						{
							await appServices.ShowNotifitionAlert(logisticsNotification);
						});
					}
				}
				else
				{
					// Instantiate the builder and set notification elements:
					Notification.Builder builder = new Notification.Builder(this)
						.SetContentTitle(title)
						.SetContentText(string.IsNullOrEmpty(body)? messageString : body)
						.SetSmallIcon(Resource.Drawable.icon)
						.SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
						.SetContentIntent(pendingIntent);

					// Build the notification:
					Notification notification = builder.Build();
					notification.Flags = NotificationFlags.AutoCancel;

					// Get the notification manager:
					NotificationManager notificationManager =
						GetSystemService(Context.NotificationService) as NotificationManager;

					// Publish the notification:
					const int notificationId = 0;
					notificationManager.Notify(notificationId, notification);
				}
			}
			else
			{
				Notification.Builder builder = new Notification.Builder(this)
					.SetContentTitle(title)
					.SetContentText(string.IsNullOrEmpty(body)? messageString : body)
					.SetSmallIcon(Resource.Drawable.icon)
					.SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate)
					.SetContentIntent(pendingIntent);

				// Build the notification:
				Notification notification = builder.Build();
				notification.Flags = NotificationFlags.AutoCancel;

				// Get the notification manager:
				NotificationManager notificationManager =
					GetSystemService(Context.NotificationService) as NotificationManager;

				// Publish the notification:
				const int notificationId = 0;
				notificationManager.Notify(notificationId, notification);
			}

			//Log.Debug(TAG, "From: " + message.From);
			//Log.Debug(TAG, "Notification Message Body: " + message.GetNotification().Body);
		}
	}
}

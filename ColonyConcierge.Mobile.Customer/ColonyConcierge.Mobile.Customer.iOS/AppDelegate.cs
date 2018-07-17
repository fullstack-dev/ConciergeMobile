using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using CarouselView.FormsPlugin.iOS;
using Plugin.Toasts;
using TK.CustomMap.iOSUnified;
using XLabs.Forms;
using FFImageLoading.Forms.Touch;
using FFImageLoading;
using Xamarin.Forms;
using ColonyConcierge.Client.iOS;
using ColonyConcierge.Client;
using System.Reflection;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using System.Globalization;
using System.Drawing;
using Firebase.CrashReporting;
using ObjCRuntime;
using CoreLocation;
using System.Net;
using Firebase.Analytics;
using UserNotifications;
using Firebase.CloudMessaging;
using Firebase.InstanceID;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;
using Newtonsoft.Json;

namespace ColonyConcierge.Mobile.Customer.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : XFormsApplicationDelegate, IUNUserNotificationCenterDelegate, IMessagingDelegate //global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public LogisticsNotification LogisticsNotification { get; set; }
		public LogisticsNotification AppLogisticsNotification { get; set; }
		private static CLLocationManager mCLLocationManager = new CLLocationManager();
		public App ApplicationForms
		{
			get;
			private set;
		}

		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			CultureInfo cultureInfo = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true);
			cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
			cultureInfo.DateTimeFormat = DateTimeFormatInfo.InvariantInfo;
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

			Client.iOS.Platform.Init();
			Appearance.Init();

			ServicePointManager.MaxServicePointIdleTime = 1000;
			ServicePointManager.DnsRefreshTimeout = 0;
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) =>
			{
				if (sender is System.Net.HttpWebRequest)
				{
					var asystyouUrl = PCLAppConfig.ConfigurationManager.AppSettings["AsystyouUrl"];
					System.Net.HttpWebRequest httpWebRequest = sender as System.Net.HttpWebRequest;
					if (httpWebRequest.RequestUri.OriginalString.Contains(asystyouUrl) ||
					   	httpWebRequest.RequestUri.OriginalString.Contains("firebase") ||
						httpWebRequest.RequestUri.OriginalString.Contains("googleapis"))
					{
						return true;
					}
					else
					{
						return false;
					}
				}
				else
				{
					return true;
				}
			};

			// Monitor token generation
			InstanceId.Notifications.ObserveTokenRefresh(TokenRefreshNotification);
			if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
			{
				// iOS 10 or later
				var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
				UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
								{
									Console.WriteLine(granted);
								});

				// For iOS 10 display notification (sent via APNS)
				UNUserNotificationCenter.Current.Delegate = this;

				// For iOS 10 data message (sent via FCM)
				Messaging.SharedInstance.RemoteMessageDelegate = this;
			}
			else
			{
				// iOS 9 or before
				var allNotificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
				var settings = UIUserNotificationSettings.GetSettingsForTypes(allNotificationTypes, null);
				UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
			}
			UIApplication.SharedApplication.RegisterForRemoteNotifications();

			Firebase.Analytics.App.Configure();

			CachedImageRenderer.Init();

			var config = new FFImageLoading.Config.Configuration()
			{
				VerboseLogging = false,
				VerbosePerformanceLogging = false,
				VerboseMemoryCacheLogging = false,
				VerboseLoadingCancelledLogging = false,
				Logger = new CustomLogger(),
			};
			ImageService.Instance.Initialize(config);

			global::Xamarin.Forms.Forms.Init();

			KeyboardOverlapRenderer.Init();

			CarouselViewRenderer.Init();
			NativePlacesApi.Init();
			Xamarin.FormsMaps.Init();
			DependencyService.Register<ToastNotificatorImplementation>(); // Register your dependency
			ToastNotificatorImplementation.Init();

			var keyNotification = new NSString("UIApplicationLaunchOptionsRemoteNotificationKey");
			if (options != null
			   && options.Keys != null
			   && options.ContainsKey(keyNotification))
			{
				NSDictionary userInfo = options.ObjectForKey(keyNotification) as NSDictionary;
				try
				{
					//JObject jsonObject = JObject.Parse(body);
					foreach (var item in userInfo)
					{
						if (item.Key.ToString().Equals("logistics"))
						{
							var jsonValue = item.Value.ToString();
							AppLogisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(jsonValue);
						}
					}
				}
				catch (Exception) { }
			}
			ApplicationForms = new App(AppLogisticsNotification);
            LoadApplication(ApplicationForms);
			ApplicationForms.Started += (sender, e) =>
			{
				mCLLocationManager.PausesLocationUpdatesAutomatically = false;
				if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
				{
					mCLLocationManager.AllowsBackgroundLocationUpdates = false;
				}
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					mCLLocationManager.RequestWhenInUseAuthorization();
				}
				mCLLocationManager.DistanceFilter = 5;
				mCLLocationManager.LocationsUpdated += (object sender2, CLLocationsUpdatedEventArgs e2) =>
				{
					var location = e2.Locations.FirstOrDefault();
					if (location != null)
					{
						DependencyService.Get<IAppServices>().LastLongitude = location.Coordinate.Longitude;
						DependencyService.Get<IAppServices>().LastLatitude = location.Coordinate.Latitude;
						DependencyService.Get<IAppServices>().RaiseAppOnLocationChanged(true);
					}
				};

				if (CLLocationManager.LocationServicesEnabled)
				{
					if (CLLocationManager.Status == CLAuthorizationStatus.Authorized
						 || CLLocationManager.Status == CLAuthorizationStatus.AuthorizedAlways
						   || CLLocationManager.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
					{
						mCLLocationManager.StartUpdatingLocation();
					}
					else
					{
						mCLLocationManager.AuthorizationChanged += (sender2, e2) =>
						{
							if (e2.Status == CLAuthorizationStatus.Authorized
								|| e2.Status == CLAuthorizationStatus.AuthorizedAlways
								|| e2.Status == CLAuthorizationStatus.AuthorizedWhenInUse)
							{
								mCLLocationManager.StartUpdatingLocation();
							}
						};
					}
				}
			};

			Connector.RequestCompleted += (sender, eventArgs) =>
			{
				RestSharp.IRestResponse _realResponse;
				try
				{
					var _realResponseField = eventArgs.RawResult.GetType().GetField("_realResponse", BindingFlags.NonPublic | BindingFlags.Instance);
					_realResponse = _realResponseField.GetValue(eventArgs.RawResult) as RestSharp.IRestResponse;
				}
				catch (Exception)
				{
					_realResponse = null;
				}

				if (_realResponse != null && _realResponse.ErrorException != null)
				{
					var webException = _realResponse.ErrorException as System.Net.WebException;
					if (webException != null &&
						(webException.Status == System.Net.WebExceptionStatus.NameResolutionFailure
						 || webException.Status == System.Net.WebExceptionStatus.ConnectFailure
						 || webException.Status == System.Net.WebExceptionStatus.ReceiveFailure
					     || webException.Status == System.Net.WebExceptionStatus.Timeout))
					{
						if (Utils.IReloadPageCurrent == null)
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								Utils.ShowErrorMessage(new CustomException(AppResources.PullDataFailMessage, webException), 4);
							});
						}
						else
						{
							Utils.IReloadPageCurrent.ShowLoadErrorPage();
						}
					}
					else if (_realResponse.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
					         || _realResponse.StatusCode == System.Net.HttpStatusCode.InternalServerError
					         || _realResponse.StatusCode == HttpStatusCode.GatewayTimeout
					         || _realResponse.StatusCode == HttpStatusCode.HttpVersionNotSupported
					         || _realResponse.StatusCode == HttpStatusCode.GatewayTimeout
					         || _realResponse.StatusCode == HttpStatusCode.NotImplemented)
					{
						if (Utils.IReloadPageCurrent == null)
						{
							Device.BeginInvokeOnMainThread(() =>
							{
								Utils.ShowErrorMessage(new CustomException(AppResources.SomethingWentWrong, webException));
							});
						}
						else
						{
							Utils.IReloadPageCurrent.ShowLoadErrorPage();
						}
					}
					//else 
					//{
					//	Device.BeginInvokeOnMainThread(() =>
					//	{
					//		Utils.ShowErrorMessage(_realResponse.ErrorException);
					//	});
					//}
				}
			};


			//if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			//{
			//	var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
			//					   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
			//					   new NSSet());

			//	UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
			//}

			//CrashReporting.Log ("Setup Crash Report");
			////// Cause crash code
			//var crash = new NSObject();
			//crash.PerformSelector (new Selector("doesNotRecognizeSelector"), crash, 0);

			return base.FinishedLaunching(app, options);
		}

		public int GetRestaurantDestinationId(string url)
		{
			int id = 0;
			if (url != null)
			{
				try
				{
					Uri uri = new Uri(url);
					var queries = new Dictionary<string, string>();
					queries = HttpUtility.ParseQueryString(uri.Query);
					if (queries.ContainsKey("destination_id"))
					{
						id = int.Parse(queries["destination_id"]);
					}
				}
				catch (Exception)
				{
					id = 0;
				}
				try
				{
					if (id < 1)
					{
						int.TryParse(url.ToLower().Replace("asystyou://restaurant/index/fitz_restaurant_school_lunch/", "").Trim(), out id);
					}
				}
				catch (Exception)
				{
					id = 0;
				}
			}
			return id;
		}

		//public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
		//{
		//	IAppServices appServices = DependencyService.Get<IAppServices>();
		//	var destinationId = GetRestaurantDestinationId(url.AbsoluteString);
		//	if (destinationId > 0)
		//	{
		//		var homePage = new HomePage(new RestaurantsTabPage(destinationId));
		//		if (ApplicationForms != null)
		//		{
		//			ApplicationForms.MainPage = homePage;
		//		}
		//		else if(Xamarin.Forms.Application.Current.MainPage != null)
		//		{
		//			Xamarin.Forms.Application.Current.MainPage = homePage;
		//		}
		//	}
		//	return false;
		//}

		//public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
		//{
		//	IAppServices appServices = DependencyService.Get<IAppServices>();
		//	var destinationId = GetRestaurantDestinationId(url.AbsoluteString);
		//	if (destinationId > 0)
		//	{
		//		var homePage = new HomePage(new RestaurantsTabPage(destinationId));
		//		if (ApplicationForms != null)
		//		{
		//			ApplicationForms.MainPage = homePage;
		//		}
		//		else if (Xamarin.Forms.Application.Current.MainPage != null)
		//		{
		//			Xamarin.Forms.Application.Current.MainPage = homePage;
		//		}
		//	}
		//	return false;
		//}

		public override void WillChange(NSKeyValueChange changeKind, NSIndexSet indexes, NSString forKey)
		{
			base.WillChange(changeKind, indexes, forKey);
		}

		public override void WillEnterForeground(UIApplication application)
		{
			base.WillEnterForeground(application);
			if (LogisticsNotification != null)
			{
				var logisticsNotification = LogisticsNotification;
				LogisticsNotification = null;
				var appServices = DependencyService.Get<IAppServices>() as AppServices;
				Device.BeginInvokeOnMainThread(async () =>
				{
					try
					{
						await appServices.ShowNotifitionAlert(logisticsNotification);
					}
					catch (Exception)
					{
						LogisticsNotification = logisticsNotification;
					}
				});
			}
		}

		public void ProcessNotification(NSDictionary userInfo)
		{
			string messageString = string.Empty;
			LogisticsNotification logisticsNotification = null;
			try
			{
				//JObject jsonObject = JObject.Parse(body);
				foreach (var item in userInfo)
				{
					if (item.Key.ToString().Equals("logistics"))
					{
						var jsonValue = item.Value.ToString();
						logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(jsonValue);
					}
					else if (item.Key.ToString().Equals("message"))
					{
						messageString = item.Value.ToString();
					}
				}
			}
			catch (Exception) { }

			if (logisticsNotification != null)
			{
				if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
				{
					Device.BeginInvokeOnMainThread(async () =>
					{
						AppServices appServices = null;
						try
						{
							appServices = DependencyService.Get<IAppServices>() as AppServices;
							await appServices.ShowNotifitionAlert(logisticsNotification);
						}
						catch (Exception)
						{
							this.LogisticsNotification = logisticsNotification;
							if (appServices != null)
							{
								appServices.LogisticsNotificationsReadOnly.Remove(logisticsNotification);
							}
						}
					});
				}
			}
			else
			{
				if (UIApplication.SharedApplication.ApplicationState != UIApplicationState.Background)
				{
					var appServices = DependencyService.Get<IAppServices>() as AppServices;
					Device.BeginInvokeOnMainThread(async () =>
					{
						await appServices.ShowNotifitionAlert(messageString);
					});
				}
			}
		}

		//public override void DidEnterBackground(UIApplication application)
		//{
		//	Messaging.SharedInstance.Disconnect();
		//	Console.WriteLine("Disconnected from FCM");
		//}

		//public override void WillEnterForeground(UIApplication application)
		//{
		//	// This method will be fired everytime a new token is generated, including the first
		//	// time. So if you need to retrieve the token as soon as it is available this is where that
		//	// should be done.
		//	var refreshedToken = InstanceId.SharedInstance.Token;

		//	if (!string.IsNullOrEmpty(refreshedToken))
		//	{
		//		// TODO: If necessary send token to application server.
		//		SendRegistrationToServer(refreshedToken);

		//		//ConnectToFCM
		//		ConnectToFCM();
		//	}
		//}

		// To receive notifications in foregroung on iOS 9 and below.
		// To receive notifications in background in any iOS version
		public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
		{
			// If you are receiving a notification message while your app is in the background,
			// this callback will not be fired 'till the user taps on the notification launching the application.

			// If you disable method swizzling, you'll need to call this method. 
			// This lets FCM track message delivery and analytics, which is performed
			// automatically with method swizzling enabled.
			//Messaging.GetInstance ().AppDidReceiveMessage (userInfo);

			ProcessNotification(userInfo);
		}

		//public override void DidEnterBackground(UIApplication uiApplication)
		//{
		//	base.DidEnterBackground
		//}

		public override void DidEnterBackground(UIApplication application)
		{
			try
			{
				Messaging.SharedInstance.Disconnect();
			}
			catch (Exception)
			{
				//if already disconnected
			}

			base.DidEnterBackground(application);
		}

		public override void OnActivated(UIApplication uiApplication)
		{
			ConnectToFCM();
			base.OnActivated(uiApplication);
		}

		// You'll need this method if you set "FirebaseAppDelegateProxyEnabled": NO in GoogleService-Info.plist
		public override void RegisteredForRemoteNotifications (UIApplication application, NSData deviceToken)
		{
			InstanceId.SharedInstance.SetApnsToken(deviceToken, ApnsTokenType.Prod);
			//InstanceId.SharedInstance.SetApnsToken(deviceToken, ApnsTokenType.Sandbox);
		}

		// To receive notifications in foreground on iOS 10 devices.
		[Export("userNotificationCenter:willPresentNotification:withCompletionHandler:")]
		public void WillPresentNotification(UNUserNotificationCenter center, UNNotification notification, Action<UNNotificationPresentationOptions> completionHandler)
		{
			if (notification != null && notification.Request.Content != null)
			{
				ProcessNotification(notification.Request.Content.UserInfo);
			}
		}

		// Receive data message on iOS 10 devices.
		public void ApplicationReceivedRemoteMessage(RemoteMessage remoteMessage)
		{
			if (remoteMessage != null)
			{
				ProcessNotification(remoteMessage.AppData);
			}
		}

		//////////////////
		////////////////// WORKAROUND
		//////////////////

		#region Workaround for handling notifications in background for iOS 10

		[Export("userNotificationCenter:didReceiveNotificationResponse:withCompletionHandler:")]
		public void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
		{
			if (response != null && response.Notification != null)
			{
				//skip for inactive iOS10
				if (AppLogisticsNotification == null)
				{
					ProcessNotification(response.Notification.Request.Content.UserInfo);
				}
				else
				{
					AppLogisticsNotification = null;
				}
			}
		}

		#endregion

		//////////////////
		////////////////// END OF WORKAROUND
		//////////////////
		/// 
		void TokenRefreshNotification(object sender, NSNotificationEventArgs e)
		{
			// This method will be fired everytime a new token is generated, including the first
			// time. So if you need to retrieve the token as soon as it is available this is where that
			// should be done.
			var refreshedToken = InstanceId.SharedInstance.Token;

			if (!string.IsNullOrEmpty(refreshedToken))
			{
				AppServices appservices = new AppServices();
				var uniqueDeviceId = appservices.UniqueDeviceId;

				// TODO: If necessary send token to application server.
				Shared.SendRegistrationToServer(refreshedToken, "ios", uniqueDeviceId);

				//ConnectToFCM
				ConnectToFCM();
			}
		}

		public static void ConnectToFCM()
		{
			try
			{
				var refreshedToken = InstanceId.SharedInstance.Token;
				if (!string.IsNullOrEmpty(refreshedToken))
				{
					Messaging.SharedInstance.Connect(error =>
					{
						if (error != null)
						{
						//ShowMessage("Unable to connect to FCM", error.LocalizedDescription, fromViewController);
						Console.WriteLine($"Unable to connect to FCM: {error}");
						}
						else
						{
						//ShowMessage("Success!", "Connected to FCM", fromViewController);
						Console.WriteLine($"Token: {InstanceId.SharedInstance.Token}");
						}
					});
				}
			}
			catch (Exception)
			{
				//if already connected
			}
		}

		public static void ShowMessage(string title, string message, UIViewController fromViewController, Action actionForOk = null)
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, (obj) =>
				{
					if (actionForOk != null)
					{
						actionForOk();
					}
				}));
				fromViewController.PresentViewController(alert, true, null);
			}
			else
			{
#pragma warning disable CS0618 // Type or member is obsolete
				new UIAlertView(title, message, null, "Ok", null).Show();
#pragma warning restore CS0618 // Type or member is obsolete
			}
		}
	}
}

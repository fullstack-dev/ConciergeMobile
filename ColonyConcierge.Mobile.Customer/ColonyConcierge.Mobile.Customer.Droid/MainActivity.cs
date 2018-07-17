using System;

using Android.App;
using Android.Content.PM;
using Android.OS;
using CarouselView.FormsPlugin.Android;
using Acr.UserDialogs;
using System.Net;
using Xamarin.Forms;
using Plugin.Toasts;
using Android.Locations;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Content;
using FFImageLoading;
using Android.Runtime;
using ColonyConcierge.Client.Droid;
using ColonyConcierge.Client;
using System.Reflection;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Android.Support.V4.Content;
using Android;
using System.Globalization;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using Firebase.Analytics;
using Firebase.Crash;
using ColonyConcierge.APIData.Data.Logistics.NotificationData;
using Newtonsoft.Json;
using ZXing.Mobile;
using System.Collections.Generic;
using Android.Support.V4.App;

namespace ColonyConcierge.Mobile.Customer.Droid
{
	[Activity(Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = false, ScreenOrientation= ScreenOrientation.Portrait, WindowSoftInputMode = SoftInput.StateAlwaysHidden | SoftInput.AdjustResize)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
	{
		public readonly int RequestPermissionsCode = 10099;
		protected const int REQUEST_CHECK_SETTINGS = 0x1;
        private bool mRequestLocationPermission = false;
		AppServices appServices;

		public static bool IsPermissionRequested = false;
		public static readonly int InstallGooglePlayServicesId = 1000;
		public static readonly string GoogleApiAvailabilityTag = typeof(GoogleApiAvailability).Name;
		private static bool IsGoogleServicesAvailability = false;
		private ErrorDialogFragment ErrorDialogFragment;
		public bool IsPaused { get; set; }
		public bool IsNeedRefresh { get; set; } = false;
		public LogisticsNotification LogisticsNotification { get; set; }

		public App ApplicationForms
		{
			get;
			private set;
		}

        private Location mLastCurrentLocation;
		public Location LastCurrentLocation
		{
			get
			{
				return mLastCurrentLocation;
			}
		}

		private GoogleApiClient mGoogleApiClient;
		public GoogleApiClient GoogleApiClient
		{
			get
			{
				return mGoogleApiClient;
			}
		}

		LocationRequest mLocationRequest;

		protected override void OnCreate(Bundle bundle)
		{
			CultureInfo cultureInfo = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.Name, true);
			cultureInfo.NumberFormat = NumberFormatInfo.InvariantInfo;
			cultureInfo.DateTimeFormat = DateTimeFormatInfo.InvariantInfo;
			CultureInfo.DefaultThreadCurrentCulture = cultureInfo;

			Platform.Init();
			Appearance.Init();

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			base.OnCreate(bundle);

			StrictMode.ThreadPolicy policy = new StrictMode.ThreadPolicy.Builder().PermitAll().Build();
			StrictMode.SetThreadPolicy(policy);

			if (Intent != null && Intent.Extras != null && Intent.Extras.ContainsKey("logistics"))
			{
				//Toast.MakeText(this, "logistics", ToastLength.Long).Show();
				//Toast.MakeText(this, Intent.Extras.GetString("logistics"), ToastLength.Long).Show();
				try
				{
					appServices = DependencyService.Get<IAppServices>() as AppServices;
					if (appServices != null && appServices.MainActivity != null && !appServices.MainActivity.IsDestroyed)
					{
						string value = string.Empty;
						try
						{
							value = Intent.Extras.GetString("logistics");
							var logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(value);
							appServices.MainActivity.IsNeedRefresh = true;
							appServices.MainActivity.LogisticsNotification = logisticsNotification;
							this.Finish();
							return;
						}
						catch (Exception)
						{
							appServices.MainActivity.Finish();
						}
					}
				}
				catch (Exception)
				{
					//App is closed, wait app init and show popup below (see enf of method)
				}
			}
			//else
			//{
			//	try
			//	{
			//		appServices = DependencyService.Get<IAppServices>() as AppServices;
			//		if (appServices != null && appServices.MainActivity != null && !appServices.MainActivity.IsDestroyed
			//		    && !appServices.MainActivity.IsFinishing)
			//		{
			//			try
			//			{
			//				this.Finish();
			//				return;
			//			}
			//			catch (Exception)
			//			{
			//				appServices.MainActivity.Finish();
			//			}
			//		}
			//	}
			//	catch (Exception)
			//	{
			//		//App is closed, wait app init and show popup below (see enf of method)
			//	}
			//}
							
			MobileBarcodeScanner.Initialize (Application);
			FFImageLoading.Forms.Droid.CachedImageRenderer.Init();

			var config = new FFImageLoading.Config.Configuration()
			{
				VerboseLogging = false,
				VerbosePerformanceLogging = false,
				VerboseMemoryCacheLogging = false,
				VerboseLoadingCancelledLogging = false,
				Logger = new CustomLogger(),
			};
			ImageService.Instance.Initialize(config);

			AppServices.MainActivityInstance = this;
			var firebaseAnalytics = FirebaseAnalytics.GetInstance(this);
			//firebaseAnalytics.SetAnalyticsCollectionEnabled(false);
			AppServices.FirebaseAnalyticsInstance = firebaseAnalytics;

			LogisticsNotification logisticsNotificationApp = null;
			Android.Net.Uri DataUri = null;
			if (Intent != null && Intent.Extras != null)
			{
				if (Intent.Extras.ContainsKey("logistics"))
				{
					string value = string.Empty;
					try
					{
						value = Intent.Extras.GetString("logistics");
						logisticsNotificationApp = JsonConvert.DeserializeObject<LogisticsNotification>(value);
						//appServices.MainActivity.LogisticsNotification = logisticsNotification;
					}
					catch (Exception)
					{
						//Toast.MakeText(this, AppResources.DataMessageError, ToastLength.Long).Show();
					}
				}
				else if(Intent.Extras.ContainsKey("DataUri"))
				{
					DataUri = (Android.Net.Uri)Intent.GetParcelableExtra("DataUri");
				}
			}

			global::Xamarin.Forms.Forms.Init(this, bundle);

			var destinationId = GetDestinationId(DataUri);
			if (destinationId > 0)
			{
				ApplicationForms = new App(destinationId);
			}
			else
			{
				ApplicationForms = new App(logisticsNotificationApp);
			}

			CarouselViewRenderer.Init();

            UserDialogs.Init(this);

			Xamarin.FormsMaps.Init(this, bundle);
			DependencyService.Register<ToastNotificatorImplementation>(); // Register your dependency
			ToastNotificatorImplementation.Init(this);

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

            LoadApplication(ApplicationForms);

			// Fix the keyboard so it doesn't overlap the grid icons above keyboard etc, and makes Android 5+ work as AdjustResize in Android 4
            Window.SetSoftInputMode(SoftInput.StateAlwaysHidden | SoftInput.AdjustResize);
			//if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
			{
				// Bug in Android 5+, this is an adequate workaround
				AndroidBug5497WorkaroundForXamarinAndroid.AssistActivity(this, WindowManager);
			}

			appServices = DependencyService.Get<IAppServices>() as AppServices;

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

				//var request = _realResponse.Request;

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

			mGoogleApiClient = new GoogleApiClient.Builder(this)
				.AddConnectionCallbacks(this)
				.AddOnConnectionFailedListener(this)
				.AddApi(Android.Gms.Location.LocationServices.API)
				.Build();

			mLocationRequest = LocationRequest.Create()
					.SetPriority(LocationRequest.PriorityHighAccuracy)
                    .SetSmallestDisplacement(5)
					.SetInterval(30 * 1000)
					.SetFastestInterval(10 * 1000);

            GetLocation(false);

            TestIfGooglePlayServicesIsInstalled();
			if (!IsPermissionRequested)
			{
				IsPermissionRequested = true;
				Handler UIHandler = new Handler();
				UIHandler.PostDelayed(() =>
				{
					try
					{
						List<string> permissions = new List<string>();
						permissions.Add(Manifest.Permission.Camera);
						permissions.Add(Manifest.Permission.ReadPhoneState);
						if (permissions.Count > 0)
						{
							ActivityCompat.RequestPermissions(this, permissions.ToArray(), RequestPermissionsCode);
						}
					}
					catch (Exception)
					{
					}
				}, 2000);
			}

			//Update notification if need
			if (Shared.IsLoggedIn)
			{
				var intent = new Intent(this, typeof(FCMRegistrationService));
				StartService(intent);
			}

			//FirebaseCrash.Report (new Exception("Crash Init"));
        }

		public int GetDestinationId(Android.Net.Uri uri)
		{
			int id = 0;
			if (uri != null)
			{
				if (uri.QueryParameterNames.Contains("destination_id"))
				{
					int.TryParse(uri.GetQueryParameter("destination_id"), out id);
				}
				else
				{
					int.TryParse(uri.ToString().ToLower().Replace("asystyou://restaurant/index/fitz_restaurant_school_lunch/", "").Trim(), out id);
				}
			}
			return id;
		}

		protected override void OnNewIntent(Intent newIntent)
		{
			base.OnNewIntent(newIntent);
			Android.Net.Uri DataUri = null;

			if (newIntent != null && newIntent.Extras != null && newIntent.Extras.KeySet().Count > 0)
			{
				if (newIntent.Extras.ContainsKey("logistics"))
				{
					AppServices newAppServices = null;
					try
					{
						newAppServices = DependencyService.Get<IAppServices>() as AppServices;
					}
					catch (Exception) { }
					string value = string.Empty;
					try
					{
						value = newIntent.Extras.GetString("logistics");
						var logisticsNotification = JsonConvert.DeserializeObject<LogisticsNotification>(value);
						if (newAppServices != null && !this.IsPaused)
						{
							this.ShowNotificationAlert(logisticsNotification);
						}
						else
						{
							this.IsNeedRefresh = true;
							this.LogisticsNotification = logisticsNotification;
						}
					}
					catch (Exception) { }
				}
				else if(newIntent.Extras.ContainsKey("DataUri"))
				{
					DataUri = (Android.Net.Uri)newIntent.GetParcelableExtra("DataUri");
				}
			}

			var destinationId = GetDestinationId(DataUri);
			if (destinationId > 0)
			{
				var homePage = new HomePage(new RestaurantsTabPage(destinationId));
				if (ApplicationForms != null)
				{
					ApplicationForms.MainPage = homePage;
				}
				else if(Xamarin.Forms.Application.Current.MainPage != null)
				{
					Xamarin.Forms.Application.Current.MainPage = homePage;
				}
			}
		}

		protected override void OnResume()
		{
			try
			{
				base.OnResume();
				IsPaused = false;

				if (!IsGoogleServicesAvailability)
				{
					IsGoogleServicesAvailability = TestIfGooglePlayServicesIsInstalled();
				}

				if (LogisticsNotification != null)
				{
					var logisticsNotification = LogisticsNotification;
					LogisticsNotification = null;
					//if (IsNeedRefresh)
					//{
					//	this.IsNeedRefresh = false;
					//this.LoadApplication(this.ApplicationForms);
					//UserDialogs.Init(() =>
					//{
					//	return (Activity)Forms.Context;
					//});
					//}
					this.ShowNotificationAlert(logisticsNotification);
				}

				var needsPermissionRequest = ZXing.Net.Mobile.Android.PermissionsHandler.NeedsPermissionRequest(this);
				if (needsPermissionRequest)
				{
					ZXing.Net.Mobile.Android.PermissionsHandler.RequestPermissionsAsync(this);
				}
			}
			catch (Exception)
			{
				IsPaused = false;
			}
		}

		protected override void OnResumeFragments()
		{
			try
			{
				if (!this.IsPaused && !this.IsFinishing && !this.IsDestroyed)
				{
					base.OnResumeFragments();
				}
			}
			catch (Exception)
			{
				this.LoadApplication(this.ApplicationForms);
				base.OnResumeFragments();
			}
		}

		public void ShowNotificationAlert(LogisticsNotification logisticsNotification)
		{
			Device.BeginInvokeOnMainThread(async () =>
			{
				try
				{
					await appServices.ShowNotifitionAlert(logisticsNotification);
				}
				catch (Exception)
				{
					this.IsNeedRefresh = true;
					this.LogisticsNotification = logisticsNotification;
				}
			});
		}


		private bool TestIfGooglePlayServicesIsInstalled()
		{
			int queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
			if (queryResult == ConnectionResult.Success)
			{
				return true;
			}

			if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
			{
				Dialog errorDialog = GoogleApiAvailability.Instance.GetErrorDialog(this, queryResult, InstallGooglePlayServicesId);
				if (ErrorDialogFragment != null)
				{
					ErrorDialogFragment.Dismiss();
				}
				ErrorDialogFragment = ErrorDialogFragment.NewInstance(errorDialog);
				ErrorDialogFragment.Cancelable = false;
				ErrorDialogFragment.Show(FragmentManager, "GooglePlayServicesDialog");
			}
			return false;
		}

		protected override void OnDestroy()
		{
			try
			{
				base.OnDestroy();
			}
			catch (Exception) { }
		}

		public override void Finish()
		{
			try
			{
				base.Finish();
			}
			catch (Exception) { }
		}

		protected override void OnPause()
		{
			base.OnPause();
			IsPaused = true;
		}

		public void OnLocationChanged(Location location)
		{
			mLastCurrentLocation = location;
			appServices.LastLatitude = location.Latitude;
			appServices.LastLongitude = location.Longitude;
			appServices.RaiseAppOnLocationChanged(true);
		}

		public void GetLocation(bool requestLocationPermission = true)
		{
            mRequestLocationPermission = requestLocationPermission;
            if (GoogleApiClient.IsConnected)
			{
                if (mRequestLocationPermission)
                {
                    try
                    {
                        LocationSettingsRequest.Builder builder = new LocationSettingsRequest.Builder().AddLocationRequest(mLocationRequest);
                        builder.SetAlwaysShow(true);
                        var result = LocationServices.SettingsApi.CheckLocationSettings(mGoogleApiClient, builder.Build());
                        result.SetResultCallback(new Android.Gms.Common.Apis.ResultCallback<LocationSettingsResult>((locationSettingsResult) =>
                        {
                            var status = locationSettingsResult.Status;
                            var state = locationSettingsResult.LocationSettingsStates;

                            switch (status.StatusCode)
                            {
                                case LocationSettingsStatusCodes.Success:
                                    // All location settings are satisfied. The client can initialize location requests here.
                                    StartLocationUpdates();
                                    break;
                                case LocationSettingsStatusCodes.ResolutionRequired:
                                    // Location settings are not satisfied. But could be fixed by showing the user a dialog.
                                    try
                                    {
                                        // Show the dialog by calling startResolutionForResult(), and check the result in onActivityResult().
                                        status.StartResolutionForResult(this, REQUEST_CHECK_SETTINGS);
                                    }
                                    catch (IntentSender.SendIntentException)
                                    {
                                        // Ignore the error.
										appServices.RaiseAppOnLocationChanged(false);
                                    }
                                    break;
                                case LocationSettingsStatusCodes.SettingsChangeUnavailable:
                                    // Location settings are not satisfied. However, we have no way to fix the
                                    // settings so we won't show the dialog.
									appServices.RaiseAppOnLocationChanged(false);
                                    break;
                            }
                        }));
                    }
                    catch (Exception) { }
                }
                else
                {
                    StartLocationUpdates();
                }	
			}
			else
			{
				GoogleApiClient.Connect();
			}
		}

		public void OnConnected(Bundle connectionHint)
		{
			GetLocation(mRequestLocationPermission);
		}

		public void OnConnectionSuspended(int cause)
		{
		}

		public void OnConnectionFailed(ConnectionResult result)
		{
		}

		public void StartLocationUpdates()
		{
			try
			{
 				LocationServices.FusedLocationApi.RequestLocationUpdates(mGoogleApiClient, mLocationRequest, this);
			}
			catch (Exception)
			{
			}
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			int requestPermissionsCode = appServices.RequestLocationPermissionsCode;
			if (requestCode == requestPermissionsCode)
			{
				// If request is cancelled, the result arrays are empty.
				string message = "";

				for (int i = 0; i < permissions.Length; i++)
				{
					if (grantResults[i] != Permission.Granted)
					{
						if (!string.IsNullOrEmpty(message))
						{
							message += "\n";
						}
						message += (permissions[i] + " is denied!");
					}
				}

				if (string.IsNullOrEmpty(message))
				{
					appServices.InvokeLocationRequestActions(true);
				}
				else
				{
					appServices.InvokeLocationRequestActions(false);
				}
			}
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			switch (requestCode)
			{
				case REQUEST_CHECK_SETTINGS:
					switch (resultCode)
					{
						case Result.Ok:
							StartLocationUpdates();
							break;
						case Result.Canceled:
							appServices.RaiseAppOnLocationChanged(false);
							break;
					}
					break;
			}
		}
	}
}


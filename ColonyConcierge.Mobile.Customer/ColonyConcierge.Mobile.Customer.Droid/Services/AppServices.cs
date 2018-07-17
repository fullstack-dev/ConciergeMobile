using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common.Apis;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Views;
using ColonyConcierge.APIData.Data;
using ColonyConcierge.Mobile.Customer.Droid;
using Com.Braintreepayments.Api;
using Com.Braintreepayments.Api.Models;
using Com.Braintreepayments.Cardform.Utils;
using CreditCardValidator;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Iid;
using Java.Lang;
using Java.Util;
using Plugin.Settings;

[assembly: Xamarin.Forms.Dependency(typeof(AppServices))]
namespace ColonyConcierge.Mobile.Customer.Droid
{
	public class AppServices : AppServicesBase
	{
		public readonly int RequestLocationPermissionsCode = 10090;

		public static MainActivity MainActivityInstance;
		public static FirebaseAnalytics FirebaseAnalyticsInstance;
		public Context Context { get; set; }

		public List<Action<bool>> Actions = new List<Action<bool>>();
		public List<Action<bool>> LocationRequestActions = new List<Action<bool>>();

		public MainActivity MainActivity
		{
			get
			{
				return MainActivityInstance;
			}
		}

		public override double LastLatitude
		{
			get;
			set;
		}

		public override double LastLongitude
		{
			get;
			set;
		}

		public override string UniqueDeviceId
		{
			get
			{
				try
				{
					var uniqueDeviceId = CrossSettings.Current.GetValueOrDefault("UniqueDeviceId", string.Empty);
					if (string.IsNullOrEmpty(uniqueDeviceId))
					{
						var telephonyDeviceID = string.Empty;
						var telephonySIMSerialNumber = string.Empty;
						TelephonyManager telephonyManager = (TelephonyManager)Context.GetSystemService(Context.TelephonyService);
						if (telephonyManager != null)
						{
							if (!string.IsNullOrEmpty(telephonyManager.DeviceId) && !string.IsNullOrEmpty(telephonyManager.DeviceId.Replace("0", "")))
							{
								telephonyDeviceID = telephonyManager.DeviceId;
							}
							else if (!string.IsNullOrEmpty(telephonyManager.SimSerialNumber))
							{
								telephonySIMSerialNumber = telephonyManager.SimSerialNumber;
							}
						}

						var androidID = Android.Provider.Settings.Secure.GetString(Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
						var deviceId = ((long)telephonyDeviceID.GetHashCode() << 32) | (long)telephonySIMSerialNumber.GetHashCode();

						using (var deviceUuid = new UUID(androidID.GetHashCode(), deviceId))
						{
							uniqueDeviceId = deviceUuid.ToString();
						}

						CrossSettings.Current.AddOrUpdateValue("UniqueDeviceId", uniqueDeviceId);
					}
					return uniqueDeviceId;
				}
				catch (System.Exception)
				{
					//App is closed or stopped
					return string.Empty;
				}
			}
		}

		public override string AppVersion
		{
			get
			{
				try
				{
					return Context.PackageManager.GetPackageInfo(
						Context.PackageName, 0).VersionName;
				}
				catch (System.Exception)
				{
					return Application.Context.PackageManager.GetPackageInfo(
						Application.Context.PackageName, PackageInfoFlags.MetaData).VersionName;
				}
			}
		}

		public override string GetRegistrationNotificationId()
		{
			//FCM
			var refreshedToken = FirebaseInstanceId.Instance.Token;
			return refreshedToken;

			//GCM
			//return GcmClient.GetRegistrationId(MainActivity
		}

		public override bool IsCanShow
		{
			get
			{
				return MainActivity != null && !MainActivity.IsDestroyed
								&& !MainActivity.IsFinishing && !MainActivity.IsPaused;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return (System.IntPtr)new System.Random().Next();
			}
		}

		public AppServices() : this(Application.Context.ApplicationContext)
		{
		}

		public AppServices(Context context)
		{
			Context = context;
		}

		public override void Vibration(int milliseconds)
		{
			try
			{
				if (MainActivity != null && !MainActivity.IsDestroyed)
				{
					Vibrator v = (Vibrator)this.MainActivity.Application.GetSystemService(Context.VibratorService);
					v.Vibrate(milliseconds);
				}
			}
			catch (System.Exception)
			{
				//not support Vibration
			}
		}

		public override void CheckLocationPermission(Action<bool> locationRequestAction)
		{
			LocationRequestActions.Add(locationRequestAction);

			//var fineLocation = ContextCompat.CheckSelfPermission(MainActivity, Manifest.Permission.AccessFineLocation);
			//var coarseLocation = ContextCompat.CheckSelfPermission(MainActivity, Manifest.Permission.AccessCoarseLocation);
			List<string> permissions = new List<string>();

			//if (coarseLocation == Android.Content.PM.Permission.Denied)
			{
				permissions.Add(Manifest.Permission.AccessCoarseLocation);
			}
			//if (fineLocation == Android.Content.PM.Permission.Denied)
			{
				permissions.Add(Manifest.Permission.AccessFineLocation);
			}
	
			//if (permissions.Count > 0)
			{
				ActivityCompat.RequestPermissions(MainActivity, permissions.ToArray(), RequestLocationPermissionsCode); 
			}
			//else 
			//{
			//	InvokeLocationRequestActions(true);
			//}
		}

		public void InvokeLocationRequestActions(bool isPermissions)
		{
			var actions = LocationRequestActions.ToList();
			LocationRequestActions.Clear();
			foreach (var action in actions)
			{
				if (action != null)
				{
					action.Invoke(isPermissions);
				}
			}
		}

		public override void GetLocation(Action<bool> action, bool isRefresh = false)
		{
			Actions.Add(action);
			if (!isRefresh && (!LastLatitude.Equals(0) || !LastLongitude.Equals(0)))
			{
				RaiseAppOnLocationChanged(true);     
			}
			else
			{
				MainActivity.GetLocation();
			}
		}

		public override void RaiseAppOnLocationChanged(bool isLocation)
		{
			var actions = Actions.ToList();
			Actions.Clear();
			foreach (var action in actions)
			{
				if (action != null)
				{
					action.Invoke(isLocation);
				}
			}
		}

		public override void TrackPage(string page)
		{
			if (MainActivity != null && !MainActivity.IsDestroyed
			   && FirebaseAnalyticsInstance != null)
			{
				Android.OS.Bundle parameters = new Android.OS.Bundle();
				parameters.PutString("page", page);
				FirebaseAnalyticsInstance.LogEvent(AnalyticsSetNameEvent, parameters);

				var parameters2 = new Android.OS.Bundle();
				parameters2.PutString("hitType", "pageview");
				FirebaseAnalyticsInstance.LogEvent(AnalyticsSendNameEvent, parameters2);
			}
		}

		public override void TrackEvent(string eventCategory, string eventAction, string eventLabel)
		{
			Android.OS.Bundle parameters = new Android.OS.Bundle();
			parameters.PutString("hitType", "event");
			parameters.PutString("eventCategory", eventCategory);
			parameters.PutString("eventAction", eventAction);
			parameters.PutString("eventLabel", eventLabel);
			FirebaseAnalyticsInstance.LogEvent(AnalyticsSendNameEvent, parameters);
		}

		public override void TrackOrder(string id, string affiliation)
		{
			Android.OS.Bundle parameters = new Android.OS.Bundle();
			parameters.PutString("transaction_id", id);
			parameters.PutString("location", affiliation);
			parameters.PutString("currency", "USD");
			FirebaseAnalyticsInstance.LogEvent(AnalyticsEcommerceNameEvent, parameters);
		}

		public override void TrackOrder(string id, string affiliation, double revenue, double shipping, double tax)
		{
			Android.OS.Bundle parameters = new Android.OS.Bundle();
			parameters.PutString("transaction_id", id);
			parameters.PutString("location", affiliation);
			parameters.PutString("currency", "USD");
			parameters.PutDouble("value", revenue);
			parameters.PutDouble("shipping", shipping);
			parameters.PutDouble("tax", tax);
			FirebaseAnalyticsInstance.LogEvent(AnalyticsEcommerceNameEvent, parameters);
		}

		public override void AddServiceNotAvaible(string id, string fullname, string email, ExtendedAddress address)
		{
			DatabaseReference mDatabase = FirebaseDatabase.Instance.Reference;
			var services = mDatabase.Root.Child("services");
			var service = services.Push();
			var values = new HashMap();
			var addressValues = new HashMap();

			//addressValues.Put("latitude", address.Latitude.ToString());
			//addressValues.Put("longitude", address.Longitude.ToString());
			if (address.BasicAddress != null)
			{
				addressValues.Put("line1", address.BasicAddress.Line1 == null? string.Empty :  address.BasicAddress.Line1);
				addressValues.Put("line2", address.BasicAddress.Line2 == null? string.Empty :  address.BasicAddress.Line2);
				addressValues.Put("line3", address.BasicAddress.Line3 == null? string.Empty :  address.BasicAddress.Line3);
				addressValues.Put("city", address.BasicAddress.City);
				addressValues.Put("state", address.BasicAddress.StateProv);
				addressValues.Put("zipcode", address.BasicAddress.ZipCode);
				addressValues.Put("country", address.BasicAddress.Country);
			}

			values.Put("id", id);
			values.Put("fullname", fullname);
			values.Put("email", email);
			values.Put("address", addressValues);
			values.Put("source", "m");
			service.SetValue(values).AddOnFailureListener(new DatabaseListener
			{
				ErrorAction = new Action<Java.Lang.Exception>((ex) => 
				{
					Android.Util.Log.Debug("FirebaseDatabase", ex.LocalizedMessage);
				})
			});
		}

		public override void GetNoncenBrainTree(BrainTreeCard brainTreeCard, Action<object> action)
		{
			BraintreeFragment mBraintreeFragment = BraintreeFragment.NewInstance(MainActivity, brainTreeCard.Token);
			PaymentMethodNonceListener paymentMethodNonceListener = new PaymentMethodNonceListener();
			paymentMethodNonceListener.Callback += (sender, e) =>
			{
				if (e is PaymentMethodNonce)
				{
					string nouce = (e as PaymentMethodNonce).Nonce;
					action(nouce);
				}
				else 
				{
					if (action != null)
					{
						action(e);
					}
				}
			};

			mBraintreeFragment.AddListener(paymentMethodNonceListener);
			var cardBuilder = new CardBuilder()
				.CardNumber(brainTreeCard.CardNumber)
				.Cvv(brainTreeCard.Cvv)
				.PostalCode(brainTreeCard.PostalCode)
				.ExpirationDate(brainTreeCard.ExpirationDate.ToString("MM/yyyy"));
			cardBuilder.Validate(false);
			Card.Tokenize(mBraintreeFragment, cardBuilder);
		}

		public override CreditCardType GetCreditCardType(string cardNumber)
		{
			var cardType = CardType.ForCardNumber(cardNumber);
			if (cardType == null || cardType == CardType.Unknown)
			{
				if (!string.IsNullOrEmpty(cardNumber))
				{
					try
					{
						var creditCardBrand = cardNumber.CreditCardBrand();
						if (creditCardBrand == CardIssuer.AmericanExpress)
						{
							return CreditCardType.Amex;
						}
						if (creditCardBrand == CardIssuer.DinersClub)
						{
							return CreditCardType.DinersClub;
						}
						if (creditCardBrand == CardIssuer.Discover)
						{
							return CreditCardType.Discover;
						}
						if (creditCardBrand == CardIssuer.JCB)
						{
							return CreditCardType.Jcb;
						}
						if (creditCardBrand == CardIssuer.Maestro)
						{
							return CreditCardType.Maestro;
						}
						if (creditCardBrand == CardIssuer.MasterCard)
						{
							return CreditCardType.Mastercard;
						}
						if (creditCardBrand == CardIssuer.Visa)
						{
							return CreditCardType.Visa;
						}
					}
					catch (System.Exception)
					{
					}
				}
				return CreditCardType.Unknown;
			}
			if (cardType == CardType.Amex)
			{
				return CreditCardType.Amex;
			}
			if (cardType == CardType.DinersClub)
			{
				return CreditCardType.Amex;
			}
			if (cardType == CardType.Discover)
			{
				return CreditCardType.Discover;
			}
			if (cardType == CardType.Jcb)
			{
				return CreditCardType.Jcb;
			}
			if (cardType == CardType.Maestro)
			{
				return CreditCardType.Maestro;
			}
			if (cardType == CardType.Mastercard)
			{
				return CreditCardType.Mastercard;
			}
			if (cardType == CardType.UnionPay)
			{
				return CreditCardType.UnionPay;
			}
			if (cardType == CardType.Visa)
			{
				return CreditCardType.Visa;
			}
			return CreditCardType.Unknown;
		}

		public override TimeZoneInfo GetTimeZoneById(string id)
		{
			return TimeZoneInfo.FindSystemTimeZoneById(id);
		}

		public override void SetNetworkBar(bool isVisible)
		{
			//nothing for Android
		}

		public override void SetShowStatus(bool isVisible)
		{
			if (MainActivity != null && !MainActivity.IsDestroyed)
			{
				if (isVisible)
				{
					MainActivity.Window.ClearFlags(WindowManagerFlags.Fullscreen);	
				}
				else
				{
					MainActivity.Window.AddFlags(WindowManagerFlags.Fullscreen);
				}
			}
		}

		public class DatabaseListener : Java.Lang.Object, Android.Gms.Tasks.IOnFailureListener, Android.Gms.Tasks.IOnSuccessListener
		{
			public Action<Java.Lang.Exception> ErrorAction
			{
				get;
				set;
			}

			public Action<Java.Lang.Object> SuccessAction
			{
				get;
				set;
			}

			public void OnFailure(Java.Lang.Exception e)
			{
				if (ErrorAction != null)
				{
					ErrorAction(e);
				}
			}

			public void OnSuccess(Java.Lang.Object result)
			{
				if (SuccessAction != null)
				{
					SuccessAction(result);
				}
			}
		}

		public class PaymentMethodNonceListener : Java.Lang.Object,
										Com.Braintreepayments.Api.Interfaces.IPaymentMethodNonceCreatedListener,
										Com.Braintreepayments.Api.Interfaces.IBraintreeCancelListener,
										//Com.Braintreepayments.Api.Interfaces.IPaymentMethodNonceCallback,
										Com.Braintreepayments.Api.Interfaces.IBraintreeErrorListener			
		{
			public event EventHandler<object> Callback;

			public void OnCancel(int p0)
			{
				if (Callback != null)
				{
					Callback(this, p0);
				}
			}

			public void OnError(Java.Lang.Exception p0)
			{
				if (Callback != null)
				{
					Callback(this, p0);
				}
			}

			public void OnPaymentMethodNonceCreated(PaymentMethodNonce p0)
			{
				if (Callback != null)
				{
					Callback(this, p0);
				}
			}
		}
	}
}

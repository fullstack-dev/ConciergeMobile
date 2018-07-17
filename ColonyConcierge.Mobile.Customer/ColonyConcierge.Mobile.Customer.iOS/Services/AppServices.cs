using System;
using System.Collections.Generic;
using System.Linq;
using ColonyConcierge.Mobile.Customer.iOS;
using BrainTree;
using CreditCardValidator;
using UIKit;
using CoreLocation;
using Foundation;
using ColonyConcierge.Mobile.Customer.Localization.Resx;
using Firebase.Analytics;
using Firebase.InstanceID;
using Plugin.Settings;
using Security;
using Firebase.Database;
using ColonyConcierge.APIData.Data;
using AudioToolbox;

[assembly: Xamarin.Forms.Dependency(typeof(AppServices))]
namespace ColonyConcierge.Mobile.Customer.iOS
{
	public class AppServices : AppServicesBase
	{
		public event EventHandler AppOnLocationChanged;

		public List<Action<bool>> Actions = new List<Action<bool>>();

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

		public override bool IsCanShow
		{
			get
			{
				return true;
			}
		}

		public override string UniqueDeviceId
		{
			get
			{
				var uniqueDeviceId = CrossSettings.Current.GetValueOrDefault("UniqueDeviceId", string.Empty);
				if (string.IsNullOrEmpty(uniqueDeviceId))
				{
					uniqueDeviceId = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
					SecRecord sr = new SecRecord(SecKind.GenericPassword);
					sr.Account = "AsystYouIdentifierForVendorID";

					SecStatusCode ssc;
					SecRecord find = SecKeyChain.QueryAsRecord(sr, out ssc);
					try
					{
						if (!string.IsNullOrEmpty(find.ValueData.ToString()))
						{
							uniqueDeviceId = find.ValueData.ToString();
						}
						else
						{
							sr.ValueData = NSData.FromString(uniqueDeviceId);
							SecKeyChain.Add(sr);
						}
					}
					catch (Exception)
					{
						sr.ValueData = NSData.FromString(uniqueDeviceId);
						SecKeyChain.Add(sr);
					}
					CrossSettings.Current.AddOrUpdateValue("UniqueDeviceId", uniqueDeviceId);
				}
				return uniqueDeviceId;
			}
		}

		public override string AppVersion
		{
			get
			{
				var _versionKey = new NSString("CFBundleShortVersionString");
				var version = NSBundle.MainBundle.InfoDictionary.ValueForKey(_versionKey);
				return string.Format("{0}", version);
			}
		}

		public override void Vibration(int milliseconds)
		{
			SystemSound.Vibrate.PlayAlertSound();
		}

		public override string GetRegistrationNotificationId()
		{
			var refreshedToken = InstanceId.SharedInstance.Token;
			return refreshedToken;
			//var deviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");
			//return deviceToke
		}

		public override void CheckLocationPermission(Action<bool> locationRequestAction)
		{
			if (locationRequestAction != null)
			{
				locationRequestAction(true);
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
				if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
				{
					string title =AppResources.LocationServicesOff;
					string message = AppResources.TurnOnLocationMessage;
					IUIAlertViewDelegate alertViewDelegate = null;
					UIAlertView uiAlertView = new UIAlertView(title, message, alertViewDelegate, AppResources.Cancel, new[] { AppResources.Settings });
					uiAlertView.Clicked += (sender, e) =>
											{
												if (e.ButtonIndex == 1)
												{
													var settingsString = UIKit.UIApplication.OpenSettingsUrlString;
													var url = new NSUrl(settingsString);
													UIApplication.SharedApplication.OpenUrl(url);
												}
												else
												{
													if (action != null)
													{
														action.Invoke(false);
													}
												}
											};
					uiAlertView.Show();
				}
				else
				{
					CLLocationManager locMgr = new CLLocationManager();
					locMgr.PausesLocationUpdatesAutomatically = false;
					if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
					{
						locMgr.RequestAlwaysAuthorization();
					}
					if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
					{
						locMgr.AllowsBackgroundLocationUpdates = true;
					}
					if (CLLocationManager.LocationServicesEnabled)
					{
						locMgr.DistanceFilter = 5;
						locMgr.LocationsUpdated += (object sender2, CLLocationsUpdatedEventArgs e2) =>
						{
							var location = e2.Locations.FirstOrDefault();
							if (location != null)
							{
								this.LastLongitude = location.Coordinate.Longitude;
								this.LastLatitude = location.Coordinate.Latitude;
                                RaiseAppOnLocationChanged(true);
							}
						};
						locMgr.StartUpdatingLocation();
					}
				}
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

			if (AppOnLocationChanged != null)
			{
				AppOnLocationChanged(this, EventArgs.Empty);
			}
		}

		public override void TrackPage(string page)
		{
			NSString[] keys = { new NSString("page") };
			NSObject[] values = { new NSString(page) };
			var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
			Analytics.LogEvent (AnalyticsSetNameEvent, parameters);

			NSString[] keys2 = { new NSString("hitType") };
			NSObject[] values2 = { new NSString("pageview") };
			var parameters2 = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values2, keys2, keys2.Length);
			Analytics.LogEvent (AnalyticsSendNameEvent, parameters2);
		}

		public override void TrackEvent(string eventCategory, string eventAction, string eventLabel)
		{
			NSString[] keys2 = { new NSString("hitType"), new NSString("eventCategory"), new NSString("eventAction"), new NSString("eventLabel") };
			NSObject[] values2 = { new NSString("event"), new NSString(eventCategory), new NSString(eventAction), new NSString(eventLabel) };
			var parameters2 = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values2, keys2, keys2.Length);
			Analytics.LogEvent(AnalyticsSendNameEvent, parameters2);
		}

		public override void TrackOrder(string id, string affiliation)
		{
			NSString[] keys = { new NSString("transaction_id"), new NSString("location"), new NSString("currency")};
			NSObject[] values = { new NSString(id), new NSString(affiliation), new NSString("USD") };
			var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
			Analytics.LogEvent(AnalyticsEcommerceNameEvent, parameters);
		}

		public override void TrackOrder(string id, string affiliation, double revenue, double shipping, double tax)
		{
			NSString[] keys = { new NSString("transaction_id"), new NSString("location"), new NSString("currency"), new NSString("value"), new NSString("shipping"), new NSString("tax") };
			NSObject[] values = { new NSString(id), new NSString(affiliation), new NSString("USD"), new NSNumber(revenue), new NSNumber(shipping), new NSNumber(tax) };
			var parameters = NSDictionary<NSString, NSObject>.FromObjectsAndKeys(values, keys, keys.Length);
			Analytics.LogEvent(AnalyticsEcommerceNameEvent, parameters);
		}

		public override void AddServiceNotAvaible(string id, string fullname, string email, ExtendedAddress address)
		{
			DatabaseReference mDatabase = Database.DefaultInstance.GetRootReference();
			var services = mDatabase.GetChild("services");
			var service = services.GetChildByAutoId();

			object[] addressKeys = { "line1", "line2", "line3", "city", "state", "zipcode", "country" };
			object[] addressValues = 
			{
				address.BasicAddress.Line1 == null? string.Empty :  address.BasicAddress.Line1,
				address.BasicAddress.Line2 == null? string.Empty :  address.BasicAddress.Line2,
				address.BasicAddress.Line3 == null? string.Empty :  address.BasicAddress.Line3,
				address.BasicAddress.City, address.BasicAddress.StateProv, address.BasicAddress.ZipCode, address.BasicAddress.Country
			};
			var addressdata = NSDictionary.FromObjectsAndKeys(addressValues, addressKeys, addressKeys.Length);

			object[] servicesKeys = { "id" , "fullname", "email", "address", "source"};
			object[] servicesValues = { id, fullname, email, addressdata, "m"};
			var data = NSDictionary.FromObjectsAndKeys(servicesValues, servicesKeys, servicesKeys.Length);

			service.ObserveSingleEvent (DataEventType.Value, (snapshot) => 
			{
			}, (error) => {
			    Console.WriteLine (error.LocalizedDescription);
			});

			service.SetValue(data);
		}

		public override void GetNoncenBrainTree(BrainTreeCard brainTreeCard, Action<object> action)
		{
			BTAPIClient BTAPIClient = new BTAPIClient(brainTreeCard.Token);
			BTCardClient BTCardClient = new BTCardClient(BTAPIClient);
			BTCard BTCard = new BTCard();
			BTCard.Number = brainTreeCard.CardNumber;
			BTCard.Cvv = brainTreeCard.Cvv;
			BTCard.PostalCode = brainTreeCard.PostalCode;
			BTCard.ExpirationYear = brainTreeCard.ExpirationDate.ToString("yyyy");
			BTCard.ExpirationMonth = brainTreeCard.ExpirationDate.ToString("MM");
			BTCard.ShouldValidate = false;

			BTCardClient.TokenizeCard(BTCard, (arg1, arg2) =>
			{
				if (arg1 != null && !string.IsNullOrEmpty(arg1.Nonce))
				{
					if (action != null)
					{
						action(arg1.Nonce);
					}
				}
				else 
				{
					if (action != null)
					{
						action(new Exception(arg2.LocalizedFailureReason + "\n" + arg2.LocalizedDescription));
					}
				}
			});
		}

		public override CreditCardType GetCreditCardType(string cardNumber)
		{
			var cardType = BTUICardType.CardTypeForNumber(cardNumber);
			if (cardType == null)
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
					catch (Exception) { }
				}
				return CreditCardType.Unknown;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_AMERICAN_EXPRESS)
			{
				return CreditCardType.Amex;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_DINERS_CLUB)
			{
				return CreditCardType.DinersClub;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_DISCOVER)
			{
				return CreditCardType.Discover;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_JCB)
			{
				return CreditCardType.Jcb;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_MAESTRO)
			{
				return CreditCardType.Maestro;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_MASTER_CARD)
			{
				return CreditCardType.Mastercard;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_UNION_PAY)
			{
				return CreditCardType.UnionPay;
			}
			if (cardType.Brand == BTUILocalizedString.CARD_TYPE_VISA)
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
			UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
		}

		public override void SetShowStatus(bool isVisible)
		{
			//nothing for iOS
		}
	}
}
